<template>
  <div>
    <!-- NAV -->
    <nav class="navbar">
      <div class="nav-logo">🔗 ShortLink</div>
      <div class="nav-links">
        <template v-if="!isLoggedIn">
          <router-link to="/login" class="btn-outline">Đăng nhập</router-link>
          <router-link to="/register" class="btn-primary">Đăng ký</router-link>
        </template>
        <template v-else>
          <span class="username">👤 {{ username }}</span>
          <button @click="logout" class="btn-logout">Đăng xuất</button>
        </template>
      </div>
    </nav>

    <!-- HERO -->
    <div class="hero">
      <h1>Rút gọn URL nhanh chóng</h1>
      <p>Chuyển đổi đường link dài thành link ngắn gọn, dễ chia sẻ</p>

      <!-- SHORT LINK BOX -->
      <div class="short-box">
        <input
          v-model="url"
          type="text"
          placeholder="Dán URL dài vào đây..."
          class="url-input"
          @keyup.enter="handleShorten"
        />
        <button @click="handleShorten" class="btn-big" :disabled="loading">
          {{ loading ? 'Đang xử lý...' : 'Rút gọn ngay' }}
        </button>
      </div>

      <!-- CẢNH BÁO CHƯA ĐĂNG NHẬP -->
      <div v-if="showWarning" class="warning-box">
         Bạn cần <router-link to="/login">đăng nhập</router-link> để sử dụng tính năng này!
      </div>

      <!-- LỖI -->
      <div v-if="errorMsg" class="error-box">
         {{ errorMsg }}
      </div>

      <!-- KẾT QUẢ SHORT LINK — hiển thị ngay bên dưới ô nhập -->
      <div v-if="shortResult" class="result-box">
        <p> Link rút gọn của bạn:</p>
        <div class="result-row">
          <a :href="shortResult" target="_blank" class="short-link">{{ shortResult }}</a>
          <button @click="copyLink" class="btn-copy">{{ copied ? ' Đã copy' : ' Copy' }}</button>
        </div>
      </div>

      <!-- DANH SÁCH LINK ĐÃ TẠO -->
      <div v-if="isLoggedIn && urls.length > 0" class="url-list">
        <h3>Các link đã tạo</h3>
        <div v-for="item in urls" :key="item.id" class="url-item">
          <div class="url-item-left">
            <a :href="buildShortUrl(item.shortCode)"
               target="_blank"
               class="short-link">
              {{ buildShortUrl(item.shortCode) }}
            </a>
            <p class="original-url">{{ item.originalUrl }}</p>
          </div>
          <span class="clicks">{{ item.clickCount ?? 0 }} clicks</span>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { urlApi, authApi } from '../api'

const url = ref('')
const shortResult = ref('')
const showWarning = ref(false)
const copied = ref(false)
const urls = ref([])
const isLoggedIn = ref(false)
const username = ref('')
const loading = ref(false)
const errorMsg = ref('')
const router = useRouter()

// Base URL của redirect service (dùng env var khi deploy)
const SHORT_BASE_URL = import.meta.env.VITE_SHORT_BASE_URL || 'https://localhost:7216'

function buildShortUrl(shortCode) {
  return `${SHORT_BASE_URL}/${shortCode}`
}

onMounted(() => {
  const savedUsername = localStorage.getItem('username')
  const savedUserId = localStorage.getItem('userId')
  if (savedUsername && savedUserId) {
    isLoggedIn.value = true
    username.value = savedUsername
    loadUrls()
  }
})

async function handleShorten() {
  showWarning.value = false
  errorMsg.value = ''

  if (!isLoggedIn.value) {
    showWarning.value = true
    return
  }
  if (!url.value.trim()) return

  loading.value = true
  try {
    const userId = localStorage.getItem('userId')
    const res = await urlApi.post('/shortcode', {
      TargetUrl: url.value.trim(),
      UserId: userId   // <-- đảm bảo đúng tên field mà backend expect
    })

    // Hiển thị link ngắn ngay bên dưới ô nhập
    shortResult.value = buildShortUrl(res.data.shortCode)
    url.value = ''
    await loadUrls()
  } catch (e) {
    console.error('Lỗi rút gọn:', e.response?.data || e.message)
    const status = e.response?.status
    if (status === 401) {
      errorMsg.value = 'Phiên đăng nhập hết hạn, vui lòng đăng nhập lại!'
      localStorage.removeItem('username')
      localStorage.removeItem('userId')
      isLoggedIn.value = false
      router.push('/login')
    } else {
      errorMsg.value = e.response?.data?.message || 'Rút gọn thất bại, thử lại!'
    }
  } finally {
    loading.value = false
  }
}

async function loadUrls() {
  try {
    const userId = localStorage.getItem('userId')
    if (!userId) return
    const res = await urlApi.get(`/shortcode/${userId}`)
    urls.value = res.data
  } catch (e) {
    console.error('Lỗi load URLs:', e)
  }
}

function copyLink() {
  navigator.clipboard.writeText(shortResult.value)
  copied.value = true
  setTimeout(() => copied.value = false, 2000)
}

async function logout() {
  try {
    await authApi.post('/auth/logout')
  } catch (e) {
    console.error('Logout error:', e)
  } finally {
    localStorage.removeItem('username')
    localStorage.removeItem('userId')
    localStorage.removeItem('role')
    isLoggedIn.value = false
    username.value = ''
    shortResult.value = ''
    urls.value = []
  }
}
</script>

<style scoped>
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 40px;
  background: white;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}
.nav-logo {
  font-size: 22px;
  font-weight: bold;
  color: #3b82f6;
}
.nav-links {
  display: flex;
  gap: 12px;
  align-items: center;
}
.btn-outline {
  padding: 8px 20px;
  border: 2px solid #3b82f6;
  color: #3b82f6;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 500;
}
.btn-outline:hover { background: #3b82f6; color: white; }
.btn-primary {
  padding: 8px 20px;
  background: #3b82f6;
  color: white;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 500;
}
.btn-primary:hover { background: #2563eb; }
.username {
  font-weight: 600;
  color: #1e293b;
  font-size: 15px;
  white-space: nowrap;
}
.btn-logout {
  padding: 8px 20px;
  background: #ef4444;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 15px;
  font-weight: 500;
}
.btn-logout:hover { background: #dc2626; }
.hero {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 70px);
  padding: 40px 20px;
  text-align: center;
}
.hero h1 { font-size: 42px; color: #1e293b; margin-bottom: 12px; }
.hero p { font-size: 18px; color: #64748b; margin-bottom: 36px; }
.short-box {
  display: flex;
  gap: 12px;
  width: 100%;
  max-width: 700px;
  align-items: center;
}
.url-input {
  flex: 1 1 auto;
  min-width: 200px;
  padding: 14px 16px;
  font-size: 16px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  outline: none;
  box-sizing: border-box;
}
.url-input:focus { border-color: #3b82f6; }
.btn-big {
  padding: 14px 28px;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  white-space: nowrap;
  flex-shrink: 0;
}
.btn-big:disabled { background: #93c5fd; cursor: not-allowed; }
.btn-big:not(:disabled):hover { background: #2563eb; }
.warning-box {
  margin-top: 20px;
  padding: 14px 24px;
  background: #fef3c7;
  border: 1px solid #fbbf24;
  border-radius: 8px;
  color: #92400e;
}
.warning-box a { color: #3b82f6; font-weight: bold; }
.error-box {
  margin-top: 20px;
  padding: 14px 24px;
  background: #fee2e2;
  border: 1px solid #fca5a5;
  border-radius: 8px;
  color: #dc2626;
  width: 100%;
  max-width: 620px;
}
.result-box {
  margin-top: 24px;
  padding: 20px 28px;
  background: #f0fdf4;
  border: 1px solid #86efac;
  border-radius: 10px;
  width: 100%;
  max-width: 620px;
  text-align: left;
}
.result-box p { color: #16a34a; font-weight: 600; margin-bottom: 10px; }
.result-row {
  display: flex;
  align-items: center;
  gap: 12px;
}
.short-link {
  color: #3b82f6;
  font-weight: bold;
  word-break: break-all;
}
.btn-copy {
  padding: 6px 16px;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  white-space: nowrap;
  font-size: 14px;
}
.btn-copy:hover { background: #2563eb; }
.url-list {
  margin-top: 40px;
  width: 100%;
  max-width: 620px;
  text-align: left;
}
.url-list h3 {
  font-size: 18px;
  color: #1e293b;
  margin-bottom: 12px;
}
.url-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 16px;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  margin-bottom: 8px;
}
.url-item-left { flex: 1; min-width: 0; }
.original-url {
  font-size: 13px;
  color: #94a3b8;
  margin-top: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 400px;
}
.clicks {
  font-size: 13px;
  color: #64748b;
  white-space: nowrap;
  margin-left: 12px;
}
</style>
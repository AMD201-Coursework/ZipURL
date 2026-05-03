<template>
  <div>
    <!-- NAV -->
    <nav class="navbar">
      <div class="nav-logo">🔗 ShortLink</div>
      <div class="nav-links">
        <!-- Chưa đăng nhập -->
        <template v-if="!isLoggedIn">
          <router-link to="/login" class="btn-outline">Đăng nhập</router-link>
          <router-link to="/register" class="btn-primary">Đăng ký</router-link>
        </template>

        <!-- Đã đăng nhập -->
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
        />
        <button @click="handleShorten" class="btn-big">Rút gọn ngay</button>
      </div>

      <!-- CẢNH BÁO CHƯA ĐĂNG NHẬP -->
      <div v-if="showWarning" class="warning-box">
        ⚠️ Bạn cần <router-link to="/login">đăng nhập</router-link> để sử dụng tính năng này!
      </div>

      <!-- KẾT QUẢ SHORT LINK -->
      <div v-if="shortResult" class="result-box">
        <p>✅ Link rút gọn của bạn:</p>
        <div class="result-row">
          <a :href="shortResult" target="_blank" class="short-link">{{ shortResult }}</a>
          <button @click="copyLink" class="btn-copy">{{ copied ? '✅ Đã copy' : '📋 Copy' }}</button>
        </div>
      </div>

      <!-- DANH SÁCH LINK ĐÃ TẠO -->
      <div v-if="isLoggedIn && urls.length > 0" class="url-list">
        <h3>Các link đã tạo</h3>
        <div v-for="item in urls" :key="item.id" class="url-item">
          <div>
            <a :href="item.shortUrl" target="_blank" class="short-link">{{ item.shortUrl }}</a>
            <p class="original-url">{{ item.originalUrl }}</p>
          </div>
          <span class="clicks">{{ item.clickCount }} clicks</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { urlApi } from '../api'

const url = ref('')
const shortResult = ref('')
const showWarning = ref(false)
const copied = ref(false)
const urls = ref([])
const isLoggedIn = ref(false)
const username = ref('')
const router = useRouter()

// Kiểm tra đăng nhập khi vào trang
onMounted(() => {
  const token = localStorage.getItem('token')
  const savedUsername = localStorage.getItem('username')
  if (token) {
    isLoggedIn.value = true
    username.value = savedUsername || 'Người dùng'
    loadUrls()
  }
})

// Rút gọn URL
async function handleShorten() {
  if (!isLoggedIn.value) {
    showWarning.value = true
    return
  }
  if (!url.value) return

  try {
    const res = await urlApi.post('/urls/shorten', { originalUrl: url.value })
    shortResult.value = res.data.shortUrl
    loadUrls()
    url.value = ''
  } catch (e) {
    alert('Rút gọn thất bại, thử lại!')
  }
}

// Trong loadUrls
async function loadUrls() {
  try {
    const res = await urlApi.get('/urls')
    urls.value = res.data
  } catch (e) {
    console.log('Lỗi load URLs:', e)
  }
}

// Load danh sách URL
async function loadUrls() {
  try {
    // --- CODE THẬT (bỏ comment khi Tín làm xong) ---
    // const res = await api.get('/urls')
    // urls.value = res.data
  } catch (e) {
    console.log('Chưa có backend')
  }
}

// Copy link
function copyLink() {
  navigator.clipboard.writeText(shortResult.value)
  copied.value = true
  setTimeout(() => copied.value = false, 2000)
}

// Đăng xuất
function logout() {
  localStorage.removeItem('token')
  localStorage.removeItem('username')
  isLoggedIn.value = false
  username.value = ''
  shortResult.value = ''
  urls.value = []
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
  margin-bottom: 0;
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
.btn-big:hover { background: #2563eb; }
.warning-box {
  margin-top: 20px;
  padding: 14px 24px;
  background: #fef3c7;
  border: 1px solid #fbbf24;
  border-radius: 8px;
  color: #92400e;
}
.warning-box a { color: #3b82f6; font-weight: bold; }
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
}
</style>
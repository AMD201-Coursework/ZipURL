<template>
  <div>
    <!-- NAV -->
    <nav class="navbar">
      <router-link to="/" class="nav-logo">🔗ShortLink</router-link>
      <div class="nav-links">
        <template v-if="!isLoggedIn">
          <router-link to="/login" class="btn-outline">Login</router-link>
          <router-link to="/register" class="btn-primary">Sign Up</router-link>
        </template>
        <template v-else>
          <div class="user-badge">
            <div class="avatar">{{ username.charAt(0).toUpperCase() }}</div>
            <span class="username">{{ username }}</span>
          </div>
          <button @click="logout" class="btn-logout">
            <span>⏻</span> Logout
          </button>
        </template>
      </div>
    </nav>

    <!-- HERO -->
    <div class="hero">
      <h1>Shorten URLs instantly</h1>
      <p>Turn long links into short, clean, and easy-to-share URLs</p>

      <!-- SHORT LINK BOX -->
      <div class="short-box">
        <input
          v-model="url"
          type="text"
          placeholder="Paste your long URL here..."
          class="url-input"
          @keyup.enter="handleShorten"
        />
        <button @click="handleShorten" class="btn-big" :disabled="loading">
          <span v-if="loading" class="spinner-dark"></span>
          {{ loading ? 'Processing...' : 'Shorten Now' }}
        </button>
      </div>

      <!-- NOT LOGGED IN WARNING -->
      <div v-if="showWarning" class="warning-box">
         You need to <router-link to="/login">log in</router-link> to use this feature!
      </div>

      <!-- ERROR -->
      <div v-if="errorMsg" class="error-box">
         {{ errorMsg }}
      </div>

      <!-- SHORT LINK RESULT -->
      <div v-if="shortResult" class="result-box">
        <p> Your shortened link:</p>
        <div class="result-row">
          <a :href="shortResult" target="_blank" class="short-link">{{ shortResult }}</a>
          <button @click="copyLink" class="btn-copy">{{ copied ? ' Copied' : ' Copy' }}</button>
        </div>
      </div>

      <!-- CREATED LINKS LIST -->
      <div v-if="isLoggedIn && urls.length > 0" class="url-list">
        <h3> Your shortened links</h3>
        <div
          v-for="item in urls"
          :key="item.id"
          class="url-item"
          :class="{ 'url-item--disabled': !item.isActive }"
        >
          <div class="url-item-left">
            <a
              :href="item.isActive ? buildShortUrl(item.shortCode) : 'javascript:void(0)'"
              :target="item.isActive ? '_blank' : '_self'"
              class="short-link"
              :class="{ 'short-link--muted': !item.isActive, 'short-link--blocked': !item.isActive }"
              @click="!item.isActive && $event.preventDefault()"
            >
              {{ buildShortUrl(item.shortCode) }}
            </a>
            <p class="original-url">{{ item.originalUrl }}</p>
          </div>

          <!-- TOGGLE SWITCH -->
          <button
            @click="toggleStatus(item)"
            class="btn-toggle"
            :class="item.isActive ? 'btn-toggle--on' : 'btn-toggle--off'"
            :disabled="item._toggling"
            :title="item.isActive ? 'Active – click to disable' : 'Inactive – click to enable'"
          >
            <span class="toggle-track">
              <span class="toggle-thumb"></span>
            </span>
            <span class="toggle-label">{{ item.isActive ? 'On' : 'Off' }}</span>
          </button>
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

const apiUrl = import.meta.env.VITE_URL_API || 'https://localhost:7216/api'
const SHORT_BASE_URL = apiUrl.replace(/\/api$/, '')

function buildShortUrl(shortCode) {
  return `${SHORT_BASE_URL}/${shortCode}`
}

onMounted(async () => {
  const savedUsername = localStorage.getItem('username')
  const savedUserId = localStorage.getItem('userId')
  if (savedUsername && savedUserId) {
    isLoggedIn.value = true
    username.value = savedUsername
    await loadUrls()
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
    const res = await urlApi.post('/shortcode/', {
      TargetUrl: url.value.trim(),
      UserId: userId
    })

    shortResult.value = buildShortUrl(res.data.shortCode)
    url.value = ''
    await loadUrls()
  } catch (e) {
    console.error('Shorten error:', e.response?.data || e.message)
    const status = e.response?.status
    if (status === 401) {
      errorMsg.value = 'Session expired, please log in again!'
      localStorage.removeItem('username')
      localStorage.removeItem('userId')
      isLoggedIn.value = false
      router.push('/login')
    } else {
      errorMsg.value = e.response?.data?.message || 'Failed to shorten URL, please try again!'
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
    // Add _toggling flag to manage loading state per button
    urls.value = res.data.map(item => ({ ...item, _toggling: false }))
  } catch (e) {
    console.error('Load URLs error:', e)
  }
}

async function toggleStatus(item) {
  item._toggling = true
  try {
    const res = await urlApi.patch(`/shortcode/${item.id}/status`)
    item.isActive = res.data.isActive
  } catch (e) {
    console.error('Toggle status error:', e)
  } finally {
    item._toggling = false
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
    router.push('/')
  }
}
</script>

<style scoped>
/* ===== NAVBAR ===== */
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 40px;
  background: white;
  box-shadow: 0 2px 12px rgba(0,0,0,0.08);
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-logo {
  font-size: 22px;
  font-weight: 800;
  color: #3b82f6;
  letter-spacing: -0.5px;
  text-decoration: none;
  cursor: pointer;
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
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.2s;
}

.btn-outline:hover {
  background: #3b82f6;
  color: white;
}

.btn-primary {
  padding: 8px 20px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  transition: opacity 0.2s;
}

.btn-primary:hover {
  opacity: 0.9;
}

.user-badge {
  display: flex;
  align-items: center;
  gap: 10px;
  background: #f1f5f9;
  border-radius: 50px;
  padding: 6px 16px 6px 6px;
  border: 1.5px solid #e2e8f0;
}

.avatar {
  width: 34px;
  height: 34px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 15px;
  flex-shrink: 0;
}

.username {
  font-weight: 600;
  color: #1e293b;
  font-size: 15px;
  white-space: nowrap;
}

.btn-logout {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 18px;
  background: #fee2e2;
  color: #dc2626;
  border: 1.5px solid #fca5a5;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.2s;
}

.btn-logout:hover {
  background: #ef4444;
  color: white;
  border-color: #ef4444;
}

/* ===== HERO ===== */
.hero {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 70px);
  padding: 40px 20px;
  text-align: center;
  background: linear-gradient(180deg, #f8faff 0%, #f1f5f9 100%);
}

.hero h1 {
  font-size: 42px;
  color: #1e293b;
  margin-bottom: 12px;
  font-weight: 800;
}

.hero p {
  font-size: 18px;
  color: #64748b;
  margin-bottom: 36px;
}

/* ===== SHORT BOX ===== */
.short-box {
  display: flex;
  gap: 12px;
  width: 100%;
  max-width: 700px;
  align-items: center;
  background: white;
  padding: 10px;
  border-radius: 14px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.1);
}

.url-input {
  flex: 1 1 auto;
  min-width: 200px;
  padding: 12px 16px;
  font-size: 16px;
  border: none;
  border-radius: 8px;
  outline: none;
  background: #f8faff;
  color: #1e293b;
  box-sizing: border-box;
}

.url-input::placeholder {
  color: #94a3b8;
}

.btn-big {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 28px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
  flex-shrink: 0;
  transition: opacity 0.2s, transform 0.1s;
}

.btn-big:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.btn-big:not(:disabled):hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.spinner-dark {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255,255,255,0.4);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  flex-shrink: 0;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* ===== BOXES ===== */
.warning-box {
  margin-top: 20px;
  padding: 14px 24px;
  background: #fef3c7;
  border: 1px solid #fbbf24;
  border-left: 4px solid #f59e0b;
  border-radius: 10px;
  color: #92400e;
  max-width: 620px;
  width: 100%;
}

.warning-box a {
  color: #3b82f6;
  font-weight: bold;
}

.error-box {
  margin-top: 20px;
  padding: 14px 24px;
  background: #fee2e2;
  border: 1px solid #fca5a5;
  border-left: 4px solid #ef4444;
  border-radius: 10px;
  color: #dc2626;
  width: 100%;
  max-width: 620px;
}

.result-box {
  margin-top: 24px;
  padding: 20px 28px;
  background: #f0fdf4;
  border: 1px solid #86efac;
  border-left: 4px solid #16a34a;
  border-radius: 12px;
  width: 100%;
  max-width: 620px;
  text-align: left;
}

.result-box p {
  color: #16a34a;
  font-weight: 700;
  margin-bottom: 10px;
}

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

.short-link--muted {
  color: #94a3b8;
  text-decoration: line-through;
}

.short-link--blocked {
  cursor: not-allowed;
  pointer-events: auto;
}

.btn-copy {
  padding: 7px 16px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  white-space: nowrap;
  font-size: 14px;
  font-weight: 500;
  transition: opacity 0.2s;
}

.btn-copy:hover {
  opacity: 0.9;
}

/* ===== URL LIST ===== */
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
  font-weight: 700;
}

.url-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 16px;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  margin-bottom: 8px;
  transition: box-shadow 0.2s, opacity 0.2s;
}

.url-item:hover {
  box-shadow: 0 4px 12px rgba(0,0,0,0.08);
}

.url-item--disabled {
  opacity: 0.55;
  background: #f8fafc;
}

.url-item-left {
  flex: 1;
  min-width: 0;
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

/* ===== TOGGLE SWITCH ===== */
.btn-toggle {
  display: flex;
  align-items: center;
  gap: 8px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 4px 0 4px 12px;
  flex-shrink: 0;
  transition: opacity 0.2s;
}

.btn-toggle:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.toggle-track {
  position: relative;
  width: 42px;
  height: 24px;
  border-radius: 12px;
  transition: background 0.25s;
  display: block;
}

.btn-toggle--on .toggle-track {
  background: #22c55e;
}

.btn-toggle--off .toggle-track {
  background: #cbd5e1;
}

.toggle-thumb {
  position: absolute;
  top: 3px;
  width: 18px;
  height: 18px;
  background: white;
  border-radius: 50%;
  box-shadow: 0 1px 4px rgba(0,0,0,0.2);
  transition: left 0.25s;
}

.btn-toggle--on .toggle-thumb {
  left: 21px;
}

.btn-toggle--off .toggle-thumb {
  left: 3px;
}

.toggle-label {
  font-size: 13px;
  font-weight: 600;
  min-width: 20px;
}

.btn-toggle--on .toggle-label {
  color: #16a34a;
}

.btn-toggle--off .toggle-label {
  color: #94a3b8;
}
</style>
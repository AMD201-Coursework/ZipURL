<template>
  <div class="page">
    <div class="card">
      <!-- Logo -->
      <div class="logo-area">
        <span class="logo-icon">🔗</span>
        <span class="logo-text">ShortLink</span>
      </div>

      <h2>Login</h2>
      <p class="sub">Welcome back!</p>

      <div class="form-group">
        <label>Email</label>
        <input v-model="form.email" type="email" placeholder="Enter your email" />
      </div>

      <div class="form-group">
        <label>Password</label>
        <div class="password-wrapper">
          <input
            v-model="form.password"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Enter your password"
          />
          <button type="button" class="eye-btn" @click="showPassword = !showPassword">
            <span v-if="showPassword">👁️</span>
            <span v-else>🙈</span>
          </button>
        </div>
      </div>

      <div v-if="error" class="error">{{ error }}</div>

      <button class="btn-primary" @click="login" :disabled="loading">
        <span v-if="loading" class="spinner"></span>
        <span>{{ loading ? 'Logging in...' : 'Login' }}</span>
      </button>

      <p class="switch">
        Don't have an account?
        <router-link to="/register">Sign up now</router-link>
      </p>

      <div class="divider"></div>

      <router-link to="/" class="btn-home">← Back to Home</router-link>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '../api'

const form = ref({ email: '', password: '' })
const error = ref('')
const loading = ref(false)
const showPassword = ref(false)
const router = useRouter()

async function login() {
  loading.value = true
  error.value = ''
  try {
    const res = await authApi.post('/auth/login', form.value)
    localStorage.setItem('username', res.data.displayName)
    localStorage.setItem('userId', res.data.userId)
    localStorage.setItem('role', res.data.role)
    localStorage.setItem('accessToken', res.data.accessToken) // Lưu token vào đây
    router.push('/home')
  } catch (e) {
    error.value = e.response?.data?.message || 'Incorrect email or password'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 24px;
}

.card {
  background: white;
  padding: 48px 52px;
  border-radius: 20px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 520px;
  display: flex;
  flex-direction: column;
}

.logo-area {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-bottom: 24px;
}

.logo-icon {
  font-size: 28px;
}

.logo-text {
  font-size: 22px;
  font-weight: 800;
  color: #3b82f6;
}

h2 {
  text-align: center;
  color: #1e293b;
  margin-bottom: 8px;
  font-size: 28px;
  font-weight: 700;
}

.sub {
  text-align: center;
  color: #64748b;
  margin-bottom: 36px;
  font-size: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #374151;
  margin-bottom: 8px;
}

.form-group input {
  padding: 14px 16px;
  font-size: 15px;
  border: 1.5px solid #d1d5db;
  border-radius: 10px;
  outline: none;
  transition: border-color 0.2s, box-shadow 0.2s;
  color: #1e293b;
  width: 100%;
}

.form-group input:focus {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15);
}

/* Password wrapper */
.password-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.password-wrapper input {
  padding-right: 48px;
}

.eye-btn {
  position: absolute;
  right: 12px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 18px;
  padding: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0.6;
  transition: opacity 0.2s;
  line-height: 1;
}

.eye-btn:hover {
  opacity: 1;
}

.error {
  background: #fee2e2;
  color: #dc2626;
  padding: 12px 16px;
  border-radius: 10px;
  margin-bottom: 16px;
  font-size: 14px;
  border-left: 4px solid #dc2626;
}

.btn-primary {
  width: 100%;
  padding: 14px;
  font-size: 16px;
  font-weight: 600;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  margin-top: 8px;
  transition: opacity 0.2s, transform 0.1s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-height: 52px;
}

.btn-primary:hover:not(:disabled) {
  opacity: 0.9;
  transform: translateY(-1px);
}

.btn-primary:disabled {
  opacity: 0.75;
  cursor: not-allowed;
}

/* Spinner */
.spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255,255,255,0.4);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
  flex-shrink: 0;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.switch {
  text-align: center;
  margin-top: 20px;
  color: #64748b;
  font-size: 14px;
}

.switch a {
  color: #3b82f6;
  font-weight: 600;
  text-decoration: none;
}

.switch a:hover {
  text-decoration: underline;
}

.divider {
  border-top: 1px solid #e5e7eb;
  margin: 24px 0 16px;
}

.btn-home {
  display: block;
  text-align: center;
  color: #64748b;
  font-size: 14px;
  text-decoration: none;
  transition: color 0.2s;
}

.btn-home:hover {
  color: #3b82f6;
}
</style>
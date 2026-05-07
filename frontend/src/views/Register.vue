<template>
  <div class="page">
    <div class="card">
      <!-- Logo -->
      <div class="logo-area">
        <span class="logo-icon">🔗</span>
        <span class="logo-text">ShortLink</span>
      </div>

      <h2>Sign Up</h2>
      <p class="sub">Create your free account today</p>

      <div class="form-group">
        <label>Display Name</label>
        <input v-model="form.displayName" type="text" placeholder="Enter your name" />
      </div>

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

      <div v-if="error" class="error">⚠️ {{ error }}</div>
      <div v-if="success" class="success">{{ success }}</div>

      <button class="btn-primary" @click="register" :disabled="loading">
        <span v-if="loading" class="spinner"></span>
        <span>{{ loading ? 'Signing up...' : 'Sign Up' }}</span>
      </button>

      <p class="switch">
        Already have an account?
        <router-link to="/login">Login</router-link>
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

const form = ref({ displayName: '', email: '', password: '' })
const error = ref('')
const success = ref('')
const loading = ref(false)
const showPassword = ref(false)
const router = useRouter()

async function register() {
  loading.value = true
  error.value = ''
  success.value = ''
  try {
    const res = await authApi.post('/auth/register', form.value)
    localStorage.setItem('username', res.data.displayName)
    localStorage.setItem('role', res.data.role)
    success.value = 'Registration successful! Redirecting...'
    setTimeout(() => router.push('/login'), 1500)
  } catch (e) {
    error.value = e.response?.data?.message || 'Registration failed, please try again!'
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

.success {
  background: #dcfce7;
  color: #16a34a;
  padding: 12px 16px;
  border-radius: 10px;
  margin-bottom: 16px;
  font-size: 14px;
  border-left: 4px solid #16a34a;
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
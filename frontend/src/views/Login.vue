<template>
  <div class="page">
    <div class="card">
      <h2>Đăng nhập</h2>
      <p class="sub">Chào mừng bạn trở lại!</p>

      <div class="form-group">
        <label>Email</label>
        <input v-model="form.email" type="email" placeholder="Nhập email của bạn" />
      </div>

      <div class="form-group">
        <label>Mật khẩu</label>
        <input v-model="form.password" type="password" placeholder="Nhập mật khẩu" />
      </div>

      <div v-if="error" class="error">{{ error }}</div>

      <button class="btn-primary" @click="login">Đăng nhập</button>

      <p class="switch">
        Chưa có tài khoản?
        <router-link to="/register">Đăng ký ngay</router-link>
      </p>

      <div class="divider"></div>

      <router-link to="/" class="btn-home">← Quay về trang chủ</router-link>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '../api'

const form = ref({ email: '', password: '' })
const error = ref('')
const router = useRouter()

async function login() {
  // GIẢ LẬP TẠM
  if (form.value.email === 'test@gmail.com' && form.value.password === '123456') {
    const res = await authApi.post('/auth/login', form.value)
    localStorage.setItem('username', res.data.displayName)
    localStorage.setItem('userId', res.data.userId)
    localStorage.setItem('role', res.data.role)
    router.push('/')
    return
  }

  try {
    const res = await authApi.post('/auth/login', form.value)
    localStorage.setItem('username', res.data.displayName)
    localStorage.setItem('role', res.data.role)
    router.push('/')
  } catch (e) {
    error.value = e.response?.data?.message || 'Email hoặc mật khẩu không đúng'
  }
}
</script>

<style scoped>
.page {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: #f1f5f9;
  padding: 24px;
}

.card {
  background: white;
  padding: 48px 52px;
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 520px;
  display: flex;
  flex-direction: column;
}

h2 {
  text-align: center;
  color: #1e293b;
  margin-bottom: 8px;
  font-size: 30px;
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
  border-radius: 8px;
  outline: none;
  transition: border-color 0.2s;
  color: #1e293b;
}

.form-group input:focus {
  border-color: #3b82f6;
}

.error {
  background: #fee2e2;
  color: #dc2626;
  padding: 12px;
  border-radius: 8px;
  margin-bottom: 16px;
  font-size: 14px;
}

.btn-primary {
  width: 100%;
  padding: 14px;
  font-size: 16px;
  font-weight: 600;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  margin-top: 8px;
  transition: background 0.2s;
}

.btn-primary:hover {
  background: #2563eb;
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
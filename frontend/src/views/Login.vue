<template>
  <div class="page">
    <div class="card">
      <h2>Đăng nhập</h2>
      <p class="sub">Chào mừng bạn trở lại!</p>

      <input v-model="form.email" type="email" placeholder="Email" />
      <input v-model="form.password" type="password" placeholder="Mật khẩu" />

      <div v-if="error" class="error">{{ error }}</div>

      <button @click="login">Đăng nhập</button>

      <p class="switch">
        Chưa có tài khoản?
        <router-link to="/register">Đăng ký ngay</router-link>
      </p>
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
    localStorage.setItem('username', 'Test User')
    localStorage.setItem('role', 'User')
    router.push('/')
    return
  }

  try {
    const res = await authApi.post('/auth/login', form.value)
    // Token nằm trong cookie tự động, chỉ lưu thông tin user
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
}
.card {
  background: white;
  padding: 40px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
}
h2 { text-align: center; color: #1e293b; margin-bottom: 6px; font-size: 26px; }
.sub { text-align: center; color: #64748b; margin-bottom: 24px; }
.error {
  background: #fee2e2;
  color: #dc2626;
  padding: 10px;
  border-radius: 6px;
  margin-bottom: 10px;
  font-size: 14px;
}
.switch { text-align: center; margin-top: 20px; color: #64748b; font-size: 14px; }
.switch a { color: #3b82f6; font-weight: bold; text-decoration: none; }
</style>
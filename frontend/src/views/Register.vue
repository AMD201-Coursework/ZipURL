<template>
  <div class="page">
    <div class="card">
      <h2>Đăng ký</h2>
      <p class="sub">Tạo tài khoản miễn phí</p>

      <input v-model="form.username" type="text" placeholder="Tên đăng nhập" />
      <input v-model="form.email" type="email" placeholder="Email" />
      <input v-model="form.password" type="password" placeholder="Mật khẩu" />

      <div v-if="error" class="error">{{ error }}</div>
      <div v-if="success" class="success">{{ success }}</div>

      <button @click="register">Đăng ký</button>

      <p class="switch">
        Đã có tài khoản?
        <router-link to="/login">Đăng nhập</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '../api'

const form = ref({ username: '', email: '', password: '' })
const error = ref('')
const success = ref('')
const router = useRouter()

async function register() {
  try {
    await authApi.post('/auth/register', form.value)
    success.value = 'Đăng ký thành công! Đang chuyển hướng...'
    setTimeout(() => router.push('/login'), 1500)
  } catch (e) {
    error.value = e.response?.data?.message || 'Đăng ký thất bại, thử lại!'
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
.success {
  background: #dcfce7;
  color: #16a34a;
  padding: 10px;
  border-radius: 6px;
  margin-bottom: 10px;
  font-size: 14px;
}
.switch { text-align: center; margin-top: 20px; color: #64748b; font-size: 14px; }
.switch a { color: #3b82f6; font-weight: bold; text-decoration: none; }
</style>
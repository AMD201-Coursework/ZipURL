<template>
  <div class="page">
    <div class="card">
      <h2>Đăng ký</h2>
      <p class="sub">Tạo tài khoản miễn phí</p>

      <div class="form-group">
        <label>Tên hiển thị</label>
        <input v-model="form.displayName" type="text" placeholder="Nhập tên của bạn" />
      </div>

      <div class="form-group">
        <label>Email</label>
        <input v-model="form.email" type="email" placeholder="Nhập email của bạn" />
      </div>

      <div class="form-group">
        <label>Mật khẩu</label>
        <input v-model="form.password" type="password" placeholder="Nhập mật khẩu" />
      </div>

      <div v-if="error" class="error">{{ error }}</div>
      <div v-if="success" class="success">{{ success }}</div>

      <button class="btn-primary" @click="register">Đăng ký</button>

      <p class="switch">
        Đã có tài khoản?
        <router-link to="/login">Đăng nhập</router-link>
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

const form = ref({ displayName: '', email: '', password: '' })
const error = ref('')
const success = ref('')
const router = useRouter()

async function register() {
  try {
    const res = await authApi.post('/auth/register', form.value)
    localStorage.setItem('username', res.data.displayName)
    localStorage.setItem('role', res.data.role)
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

.success {
  background: #dcfce7;
  color: #16a34a;
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
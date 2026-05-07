<template>
  <div class="landing">

    <!-- NAVBAR -->
    <nav class="navbar">
      <div class="nav-logo">🔗 ShortLinkTKT</div>
      <div class="nav-links">
        <!-- Chưa đăng nhập: hiện 2 nút -->
        <template v-if="!isLoggedIn">
          <router-link to="/login" class="btn-outline">Đăng nhập</router-link>
          <router-link to="/register" class="btn-primary">Đăng ký miễn phí</router-link>
        </template>
        <!-- Đã đăng nhập: hiện avatar + tên -->
        <template v-else>
          <div class="user-badge">
            <div class="avatar">{{ username.charAt(0).toUpperCase() }}</div>
            <span class="username-text">{{ username }}</span>
          </div>
        </template>
      </div>
    </nav>

    <!-- HERO SECTION -->
    <section class="hero-section">
      <div class="hero-content">
        <div class="badge">✨ Rút gọn link miễn phí</div>
        <h1>Rút gọn URL<br /><span class="gradient-text">nhanh chóng & thông minh</span></h1>
        <p>
          Chuyển đổi những đường link dài thành link ngắn gọn, dễ nhớ và dễ chia sẻ
          trên mọi nền tảng. Theo dõi lượt click realtime.
        </p>
        <div class="hero-actions">
          <!-- Đã đăng nhập: chỉ hiện nút Bắt đầu -> /home -->
          <template v-if="isLoggedIn">
            <router-link to="/home" class="cta-btn">🚀 Bắt đầu ngay</router-link>
          </template>
          <!-- Chưa đăng nhập: hiện cả 2 nút -->
          <template v-else>
            <router-link to="/register" class="cta-btn">🚀 Bắt đầu miễn phí</router-link>
            <router-link to="/login" class="cta-btn-ghost">Đăng nhập</router-link>
          </template>
        </div>
        <!-- Demo visual -->
        <div class="demo-box">
          <div class="demo-row">
            <span class="demo-label">🔗 URL gốc</span>
            <span class="demo-url long">https://example.com/very/long/path/to/some/page?ref=social&campaign=summer2024</span>
          </div>
          <div class="demo-arrow">↓</div>
          <div class="demo-row">
            <span class="demo-label">✂️ Sau rút gọn</span>
            <span class="demo-url short">shortlnk.io/aB3xZ</span>
          </div>
        </div>
      </div>
    </section>

    <!-- HOW IT WORKS -->
    <section class="how-section">
      <h2>Cách sử dụng</h2>
      <p class="section-sub">Chỉ 3 bước đơn giản</p>
      <div class="steps-grid">
        <div class="step-card">
          <div class="step-num">1</div>
          <h3>Đăng ký tài khoản</h3>
          <p>Tạo tài khoản miễn phí chỉ với email và mật khẩu.</p>
        </div>
        <div class="step-arrow">→</div>
        <div class="step-card">
          <div class="step-num">2</div>
          <h3>Dán URL cần rút gọn</h3>
          <p>Copy link dài và dán vào ô nhập trên trang chủ.</p>
        </div>
        <div class="step-arrow">→</div>
        <div class="step-card">
          <div class="step-num">3</div>
          <h3>Nhận & chia sẻ link</h3>
          <p>Copy link ngắn và chia sẻ lên mạng xã hội, tin nhắn.</p>
        </div>
      </div>
    </section>

    <!-- CTA SECTION -->
    <section class="cta-section">
      <h2>Sẵn sàng bắt đầu?</h2>
      <p>Đăng ký miễn phí ngay hôm nay và rút gọn link đầu tiên của bạn!</p>
      <router-link v-if="isLoggedIn" to="/home" class="cta-btn-white">🚀 Bắt đầu ngay</router-link>
      <router-link v-else to="/register" class="cta-btn-white">🚀 Tạo tài khoản miễn phí</router-link>
    </section>

    <!-- FOOTER -->
    <footer class="footer">
      <div class="footer-logo">🔗 ShortLink</div>
      <p>© 2024 ShortLink. Tất cả quyền được bảo lưu.</p>
    </footer>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const isLoggedIn = ref(false)
const username = ref('')

onMounted(() => {
  const savedUsername = localStorage.getItem('username')
  const savedUserId = localStorage.getItem('userId')
  if (savedUsername && savedUserId) {
    isLoggedIn.value = true
    username.value = savedUsername
  }
})
</script>

<style scoped>
.landing {
  font-family: Arial, sans-serif;
}

/* ===== NAVBAR ===== */
.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 60px;
  background: white;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-logo {
  font-size: 22px;
  font-weight: 800;
  color: #3b82f6;
}

.nav-links {
  display: flex;
  gap: 12px;
  align-items: center;
}

.btn-outline {
  padding: 9px 22px;
  border: 2px solid #3b82f6;
  color: #3b82f6;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  transition: all 0.2s;
  font-size: 15px;
}

.btn-outline:hover {
  background: #3b82f6;
  color: white;
}

.btn-primary {
  padding: 9px 22px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border-radius: 8px;
  text-decoration: none;
  font-weight: 600;
  font-size: 15px;
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

.username-text {
  font-weight: 600;
  color: #1e293b;
  font-size: 15px;
  white-space: nowrap;
}

/* ===== HERO SECTION ===== */
.hero-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 100px 60px 80px;
  text-align: center;
  color: white;
}

.hero-content {
  max-width: 800px;
  margin: 0 auto;
}

.badge {
  display: inline-block;
  background: rgba(255,255,255,0.2);
  border: 1px solid rgba(255,255,255,0.4);
  padding: 6px 18px;
  border-radius: 50px;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 28px;
  backdrop-filter: blur(4px);
}

.hero-section h1 {
  font-size: 52px;
  font-weight: 800;
  margin-bottom: 20px;
  line-height: 1.2;
}

.gradient-text {
  background: linear-gradient(135deg, #fbbf24, #f472b6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.hero-section p {
  font-size: 18px;
  opacity: 0.9;
  margin-bottom: 36px;
  line-height: 1.7;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.hero-actions {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-bottom: 52px;
  flex-wrap: wrap;
}

.cta-btn {
  display: inline-block;
  padding: 16px 36px;
  background: white;
  color: #3b82f6;
  border-radius: 12px;
  text-decoration: none;
  font-size: 16px;
  font-weight: 700;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.cta-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(0,0,0,0.2);
}

.cta-btn-ghost {
  display: inline-block;
  padding: 16px 36px;
  border: 2px solid rgba(255,255,255,0.7);
  color: white;
  border-radius: 12px;
  text-decoration: none;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.2s;
}

.cta-btn-ghost:hover {
  background: rgba(255,255,255,0.15);
}

/* Demo box */
.demo-box {
  background: rgba(255,255,255,0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255,255,255,0.3);
  border-radius: 16px;
  padding: 28px 32px;
  text-align: left;
  max-width: 600px;
  margin: 0 auto;
}

.demo-row {
  display: flex;
  align-items: flex-start;
  gap: 14px;
}

.demo-label {
  font-size: 12px;
  font-weight: 700;
  opacity: 0.8;
  white-space: nowrap;
  min-width: 90px;
  padding-top: 2px;
}

.demo-url {
  font-family: monospace;
  font-size: 14px;
  background: rgba(0,0,0,0.2);
  padding: 8px 12px;
  border-radius: 8px;
  word-break: break-all;
  flex: 1;
}

.demo-url.short {
  color: #fbbf24;
  font-weight: 700;
  font-size: 16px;
}

.demo-arrow {
  text-align: center;
  font-size: 22px;
  opacity: 0.7;
  margin: 10px 0;
  padding-left: 104px;
}

/* ===== HOW IT WORKS ===== */
.how-section {
  background: white;
  padding: 80px 60px;
  text-align: center;
}

.how-section h2 {
  font-size: 36px;
  font-weight: 800;
  color: #1e293b;
  margin-bottom: 10px;
}

.section-sub {
  color: #64748b;
  font-size: 16px;
  margin-bottom: 52px;
}

.steps-grid {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 20px;
  max-width: 900px;
  margin: 0 auto;
  flex-wrap: wrap;
}

.step-card {
  background: #f8faff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  padding: 32px 28px;
  flex: 1;
  min-width: 200px;
  max-width: 260px;
}

.step-num {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #3b82f6, #6366f1);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  font-weight: 800;
  margin: 0 auto 16px;
}

.step-card h3 {
  font-size: 17px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 10px;
}

.step-card p {
  color: #64748b;
  font-size: 14px;
  line-height: 1.6;
}

.step-arrow {
  font-size: 28px;
  color: #3b82f6;
  font-weight: bold;
  flex-shrink: 0;
}

/* ===== CTA SECTION ===== */
.cta-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 80px 60px;
  text-align: center;
  color: white;
}

.cta-section h2 {
  font-size: 36px;
  font-weight: 800;
  margin-bottom: 14px;
}

.cta-section p {
  font-size: 18px;
  opacity: 0.9;
  margin-bottom: 36px;
}

.cta-btn-white {
  display: inline-block;
  padding: 18px 48px;
  background: white;
  color: #6366f1;
  border-radius: 14px;
  text-decoration: none;
  font-size: 17px;
  font-weight: 700;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.cta-btn-white:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(0,0,0,0.2);
}

/* ===== FOOTER ===== */
.footer {
  background: #0f172a;
  padding: 32px 60px;
  text-align: center;
  color: #64748b;
}

.footer-logo {
  font-size: 20px;
  font-weight: 800;
  color: #3b82f6;
  margin-bottom: 8px;
}

.footer p {
  font-size: 14px;
}

/* ===== RESPONSIVE ===== */
@media (max-width: 768px) {
  .navbar { padding: 14px 20px; }
  .hero-section { padding: 60px 20px; }
  .hero-section h1 { font-size: 32px; }
  .step-arrow { transform: rotate(90deg); }
  .steps-grid { flex-direction: column; }
  .how-section, .cta-section { padding: 60px 20px; }
}
</style>
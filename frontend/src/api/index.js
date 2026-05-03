import axios from 'axios'

// API đăng nhập, đăng ký - Khỏe làm (port 5230)
export const authApi = axios.create({
  baseURL: 'http://localhost:5230/api',
  withCredentials: true, // ← QUAN TRỌNG: cho phép gửi/nhận cookie
})

// API short URL - Tín làm (port 5081)
export const urlApi = axios.create({
  baseURL: 'http://localhost:5081/api',
  withCredentials: true,
})

// KHÔNG cần attachToken nữa vì token nằm trong cookie tự động
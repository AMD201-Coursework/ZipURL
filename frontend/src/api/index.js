import axios from 'axios'

// API đăng nhập, đăng ký - Khỏe làm (port 5230)
export const authApi = axios.create({
  baseURL: 'http://localhost:5230/api',
})

// API short URL - Tín làm (port 5081)
export const urlApi = axios.create({
  baseURL: 'http://localhost:5081/api',
})

// Tự động gắn token vào mỗi request
const attachToken = (config) => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
}

authApi.interceptors.request.use(attachToken)
urlApi.interceptors.request.use(attachToken)
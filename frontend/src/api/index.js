import axios from 'axios'

// Use environment variables for easy deployment
// Create a .env file at the frontend root:
//   VITE_AUTH_API=https://your-auth-api.com/api
//   VITE_URL_API=https://your-url-api.com/api
//
// When deploying to Vercel, add these variables in the Vercel dashboard

export const authApi = axios.create({
  baseURL: import.meta.env.VITE_AUTH_API || 'https://localhost:7028/api',
  withCredentials: true,
})

export const urlApi = axios.create({
  baseURL: import.meta.env.VITE_URL_API || 'https://localhost:7216/api',
  withCredentials: true,
})

// Tự động thêm Token vào Header cho các request
const addAuthToken = (config) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}

authApi.interceptors.request.use(addAuthToken)
urlApi.interceptors.request.use(addAuthToken)
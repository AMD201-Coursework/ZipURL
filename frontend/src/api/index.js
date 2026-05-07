import axios from 'axios'
//   VITE_AUTH_API=https://localhost:7028/api
//   VITE_URL_API=https://localhost:7216/api

export const authApi = axios.create({
  baseURL: import.meta.env.VITE_AUTH_API || 'https://localhost:7028/api',
  withCredentials: true,
})

export const urlApi = axios.create({
  baseURL: import.meta.env.VITE_URL_API || 'https://localhost:7216/api',
  withCredentials: true,
})
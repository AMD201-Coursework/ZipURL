import axios from 'axios'

export const authApi = axios.create({
  baseURL: 'https://localhost:7028/api',
  withCredentials: true,
})

export const urlApi = axios.create({
  baseURL: 'https://localhost:7216/api',
  withCredentials: true,
})
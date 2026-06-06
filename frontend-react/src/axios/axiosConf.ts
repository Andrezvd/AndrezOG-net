import axios from 'axios'
import type { AxiosInstance } from 'axios'



const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5201/api'

const api: AxiosInstance = axios.create({
	baseURL: API_URL,
	withCredentials: true,
	headers: {
		'Content-Type': 'application/json',
	},
})

// Adjunta el token JWT si existe
api.interceptors.request.use(
	(config) => {
		const token = localStorage.getItem('token')
		if (token && config.headers) {
			// eslint-disable-next-line @typescript-eslint/ban-ts-comment
			// @ts-ignore
			config.headers.Authorization = `Bearer ${token}`
		}
		return config
	},
	(error) => Promise.reject(error)
)

export function setAuthToken(token: string | null) {
	if (token) localStorage.setItem('token', token)
	else localStorage.removeItem('token')
}

export default api


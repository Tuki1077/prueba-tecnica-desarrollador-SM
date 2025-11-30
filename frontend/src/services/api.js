import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Interceptor para agregar el token a todas las peticiones
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Interceptor para manejar errores de autenticación
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Auth
export const login = async (nombreUsuario, password) => {
  const response = await api.post('/auth/login', { nombreUsuario, password })
  if (response.data.token) {
    localStorage.setItem('token', response.data.token)
    localStorage.setItem('user', JSON.stringify({
      usuarioId: response.data.usuarioId,
      nombreUsuario: response.data.nombreUsuario,
      rol: response.data.rol
    }))
  }
  return response.data
}

// Tiendas
export const obtenerTiendas = async () => {
  const response = await api.get('/tiendas')
  return response.data
}

export const obtenerTiendasPorPais = async (codigoPais) => {
  const response = await api.get(`/tiendas/pais/${codigoPais}`)
  return response.data
}

// Proyecciones
export const obtenerResumenProyeccion = async (anio, tiendaId) => {
  const response = await api.get(`/proyecciones/${anio}/${tiendaId}`)
  return response.data
}

export const crearProyeccion = async (proyeccion) => {
  const response = await api.post('/proyecciones', proyeccion)
  return response.data
}

export const actualizarProyeccion = async (proyeccion) => {
  const response = await api.put('/proyecciones', proyeccion)
  return response.data
}

export const cerrarProyeccion = async (anio, tiendaId) => {
  const response = await api.post(`/proyecciones/cerrar/${anio}/${tiendaId}`)
  return response.data
}

export const obtenerCrecimiento = async (tiendaId) => {
  const response = await api.get(`/proyecciones/crecimiento/${tiendaId}`)
  return response.data
}

// Bitácora
export const obtenerBitacoras = async (params) => {
  const response = await api.get('/bitacora', { params })
  return response.data
}

export default api

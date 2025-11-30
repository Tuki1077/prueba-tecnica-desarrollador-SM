import { useState } from 'react'
import { login } from '../services/api'
import './Login.css'

function Login({ onLogin }) {
  const [nombreUsuario, setNombreUsuario] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    try {
      const data = await login(nombreUsuario, password)
      onLogin({
        usuarioId: data.usuarioId,
        nombreUsuario: data.nombreUsuario,
        rol: data.rol
      })
    } catch (err) {
      setError(err.response?.data?.message || 'Error al iniciar sesión')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <h2>Los Ladrillos S.A.</h2>
        <p className="subtitle">Sistema de Proyección de Ventas</p>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="nombreUsuario">Usuario</label>
            <input
              type="text"
              id="nombreUsuario"
              value={nombreUsuario}
              onChange={(e) => setNombreUsuario(e.target.value)}
              required
              disabled={loading}
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Contraseña</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={loading}
            />
          </div>
          {error && <div className="error-message">{error}</div>}
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Ingresando...' : 'Ingresar'}
          </button>
        </form>
      </div>
    </div>
  )
}

export default Login

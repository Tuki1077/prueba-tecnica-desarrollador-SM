import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { useState, useEffect } from 'react'
import Login from './components/Login'
import ProyeccionVentas from './components/ProyeccionVentas'
import Bitacora from './components/Bitacora'
import './styles/global.css'

function App() {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('token')
    const userData = localStorage.getItem('user')
    if (token && userData) {
      setUser(JSON.parse(userData))
    }
    setLoading(false)
  }, [])

  const handleLogin = (userData) => {
    setUser(userData)
  }

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    setUser(null)
  }

  if (loading) {
    return <div className="loading">Cargando...</div>
  }

  return (
    <BrowserRouter>
      <div className="app">
        {user && (
          <header className="app-header">
            <div className="header-content">
              <h1>Los Ladrillos S.A. - Proyección de Ventas</h1>
              <div className="user-info">
                <span>Usuario: {user.nombreUsuario}</span>
                <span>Rol: {user.rol}</span>
                <button onClick={handleLogout} className="btn-logout">Cerrar Sesión</button>
              </div>
            </div>
          </header>
        )}
        <main className="app-main">
          <Routes>
            <Route path="/login" element={
              !user ? <Login onLogin={handleLogin} /> : <Navigate to="/" replace />
            } />
            <Route path="/" element={
              user ? <ProyeccionVentas user={user} /> : <Navigate to="/login" replace />
            } />
            <Route path="/bitacora" element={
              user ? <Bitacora user={user} /> : <Navigate to="/login" replace />
            } />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  )
}

export default App

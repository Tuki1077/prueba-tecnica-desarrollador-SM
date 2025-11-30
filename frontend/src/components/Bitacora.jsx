import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { obtenerBitacoras } from '../services/api'

function Bitacora() {
  const navigate = useNavigate()
  const [bitacoras, setBitacoras] = useState([])
  const [loading, setLoading] = useState(false)
  const [filtros, setFiltros] = useState({
    fechaInicio: '',
    fechaFin: '',
    accion: ''
  })

  useEffect(() => {
    cargarBitacoras()
  }, [])

  const cargarBitacoras = async () => {
    setLoading(true)
    try {
      const params = {}
      if (filtros.fechaInicio) params.fechaInicio = filtros.fechaInicio
      if (filtros.fechaFin) params.fechaFin = filtros.fechaFin
      if (filtros.accion) params.accion = filtros.accion

      const data = await obtenerBitacoras(params)
      setBitacoras(data)
    } catch (err) {
      console.error('Error al cargar bitácoras:', err)
    } finally {
      setLoading(false)
    }
  }

  const formatearFecha = (fecha) => {
    return new Date(fecha).toLocaleString('es-GT', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    })
  }

  const handleFiltrar = () => {
    cargarBitacoras()
  }

  const handleLimpiar = () => {
    setFiltros({
      fechaInicio: '',
      fechaFin: '',
      accion: ''
    })
    setTimeout(() => cargarBitacoras(), 100)
  }

  return (
    <div className="bitacora-container">
      <div className="bitacora-header">
        <h2>Bitácora de Movimientos</h2>
        <button onClick={() => navigate('/')} className="btn-secondary">
          Volver a Proyecciones
        </button>
      </div>

      <div className="filtros-container">
        <div className="filtro-group">
          <label>Fecha Inicio:</label>
          <input
            type="date"
            value={filtros.fechaInicio}
            onChange={(e) => setFiltros({ ...filtros, fechaInicio: e.target.value })}
          />
        </div>
        <div className="filtro-group">
          <label>Fecha Fin:</label>
          <input
            type="date"
            value={filtros.fechaFin}
            onChange={(e) => setFiltros({ ...filtros, fechaFin: e.target.value })}
          />
        </div>
        <div className="filtro-group">
          <label>Acción:</label>
          <select 
            value={filtros.accion}
            onChange={(e) => setFiltros({ ...filtros, accion: e.target.value })}
          >
            <option value="">Todas</option>
            <option value="LOGIN">LOGIN</option>
            <option value="INSERT">INSERT</option>
            <option value="UPDATE">UPDATE</option>
            <option value="DELETE">DELETE</option>
            <option value="CIERRE">CIERRE</option>
          </select>
        </div>
        <div className="filtro-acciones">
          <button onClick={handleFiltrar} className="btn-primary">Filtrar</button>
          <button onClick={handleLimpiar} className="btn-secondary">Limpiar</button>
        </div>
      </div>

      {loading ? (
        <div className="loading">Cargando bitácoras...</div>
      ) : (
        <div className="tabla-container">
          <table className="tabla-bitacora">
            <thead>
              <tr>
                <th>ID</th>
                <th>Fecha/Hora</th>
                <th>Usuario</th>
                <th>Acción</th>
                <th>Tabla</th>
                <th>Registro ID</th>
                <th>IP</th>
                <th>Valores Anteriores</th>
                <th>Valores Nuevos</th>
              </tr>
            </thead>
            <tbody>
              {bitacoras.length === 0 ? (
                <tr>
                  <td colSpan="9" className="sin-datos">No hay registros en la bitácora</td>
                </tr>
              ) : (
                bitacoras.map((bitacora) => (
                  <tr key={bitacora.bitacoraId}>
                    <td>{bitacora.bitacoraId}</td>
                    <td>{formatearFecha(bitacora.fechaHora)}</td>
                    <td>{bitacora.nombreUsuario}</td>
                    <td>
                      <span className={`accion-badge accion-${bitacora.accion.toLowerCase()}`}>
                        {bitacora.accion}
                      </span>
                    </td>
                    <td>{bitacora.tabla}</td>
                    <td>{bitacora.registroId || '-'}</td>
                    <td>{bitacora.direccionIP || '-'}</td>
                    <td className="valores-cell">{bitacora.valoresAnteriores || '-'}</td>
                    <td className="valores-cell">{bitacora.valoresNuevos || '-'}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

export default Bitacora

import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  obtenerTiendasPorPais,
  obtenerResumenProyeccion,
  actualizarProyeccion,
  cerrarProyeccion
} from '../services/api'
import './ProyeccionVentas.css'

const MESES = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
]

function ProyeccionVentas({ user }) {
  const navigate = useNavigate()
  const [tiendas, setTiendas] = useState([])
  const [tiendaSeleccionada, setTiendaSeleccionada] = useState(null)
  const [anio, setAnio] = useState(2026)
  const [resumen, setResumen] = useState(null)
  const [loading, setLoading] = useState(false)
  const [editando, setEditando] = useState({})
  const [error, setError] = useState('')
  const [mensaje, setMensaje] = useState('')

  useEffect(() => {
    cargarTiendas()
  }, [])

  const cargarTiendas = async () => {
    try {
      const data = await obtenerTiendasPorPais('GT')
      setTiendas(data)
      if (data.length > 0) {
        setTiendaSeleccionada(data[0].tiendaId)
      }
    } catch (err) {
      setError('Error al cargar tiendas')
    }
  }

  const cargarResumen = async () => {
    if (!tiendaSeleccionada) return

    setLoading(true)
    setError('')
    try {
      const data = await obtenerResumenProyeccion(anio, tiendaSeleccionada)
      setResumen(data)
    } catch (err) {
      setError(err.response?.data?.message || 'Error al cargar proyección')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (tiendaSeleccionada) {
      cargarResumen()
    }
  }, [tiendaSeleccionada, anio])

  const handleEditarMonto = (proyeccionId, montoActual) => {
    setEditando({ ...editando, [proyeccionId]: montoActual })
  }

  const handleCancelarEdicion = (proyeccionId) => {
    const newEditando = { ...editando }
    delete newEditando[proyeccionId]
    setEditando(newEditando)
  }

  const handleGuardarMonto = async (proyeccionId) => {
    try {
      await actualizarProyeccion({
        proyeccionVentaId: proyeccionId,
        montoProyectado: parseFloat(editando[proyeccionId])
      })
      setMensaje('Proyección actualizada exitosamente')
      setTimeout(() => setMensaje(''), 3000)
      handleCancelarEdicion(proyeccionId)
      cargarResumen()
    } catch (err) {
      setError(err.response?.data?.message || 'Error al actualizar proyección')
    }
  }

  const handleCerrar = async () => {
    if (!window.confirm('¿Está seguro de cerrar esta proyección? No podrá modificarla después.')) {
      return
    }

    try {
      await cerrarProyeccion(anio, tiendaSeleccionada)
      setMensaje('Proyección cerrada exitosamente')
      setTimeout(() => setMensaje(''), 3000)
      cargarResumen()
    } catch (err) {
      setError(err.response?.data?.message || 'Error al cerrar proyección')
    }
  }

  const formatearMoneda = (valor) => {
    return new Intl.NumberFormat('es-GT', {
      style: 'currency',
      currency: 'GTQ',
      minimumFractionDigits: 2
    }).format(valor || 0)
  }

  const formatearPorcentaje = (valor) => {
    if (valor == null) return 'N/A'
    return `${valor >= 0 ? '+' : ''}${valor.toFixed(2)}%`
  }

  const getPorcentajeClass = (valor) => {
    if (valor == null) return ''
    return valor >= 0 ? 'positivo' : 'negativo'
  }

  if (!resumen) {
    return <div className="loading">Cargando...</div>
  }

  const esCerrado = resumen.estado === 'CERRADO'

  return (
    <div className="proyeccion-container">
      <div className="proyeccion-header">
        <div className="controles">
          <div className="control-group">
            <label>Año:</label>
            <select value={anio} onChange={(e) => setAnio(parseInt(e.target.value))} className="select-anio">
              <option value="2026">2026</option>
            </select>
          </div>
          <div className="control-group">
            <label>Tienda:</label>
            <select 
              value={tiendaSeleccionada || ''} 
              onChange={(e) => setTiendaSeleccionada(parseInt(e.target.value))}
              className="select-tienda"
            >
              {tiendas.map(t => (
                <option key={t.tiendaId} value={t.tiendaId}>
                  {t.codigoTienda}-{t.nombreTienda}
                </option>
              ))}
            </select>
          </div>
          <div className="control-group">
            <label>Moneda:</label>
            <span className="moneda-display">Local ({resumen.moneda})</span>
          </div>
          <div className="control-group">
            <label>Estado:</label>
            <span className={`estado-badge ${esCerrado ? 'cerrado' : 'abierto'}`}>
              {resumen.estado}
            </span>
          </div>
        </div>
        <div className="acciones">
          <button onClick={() => navigate('/bitacora')} className="btn-secondary">
            Ver Bitácora
          </button>
          {!esCerrado && (
            <button onClick={handleCerrar} className="btn-cerrar">
              Cerrar
            </button>
          )}
        </div>
      </div>

      {error && <div className="mensaje-error">{error}</div>}
      {mensaje && <div className="mensaje-exito">{mensaje}</div>}

      <div className="tabla-container">
        <table className="tabla-proyeccion">
          <thead>
            <tr>
              <th rowSpan="2">Mes</th>
              <th colSpan="3">Ventas</th>
              <th rowSpan="2">% Crecimiento</th>
            </tr>
            <tr>
              <th>Ventas 2024</th>
              <th>Ventas 2025</th>
              <th>Proyección 2026</th>
            </tr>
          </thead>
          <tbody>
            {resumen.meses.map((mes, idx) => (
              <tr key={idx}>
                <td className="mes-nombre">{mes.nombreMes}</td>
                <td className="monto">{formatearMoneda(mes.venta2024)}</td>
                <td className="monto">{formatearMoneda(mes.venta2025)}</td>
                <td className="monto">
                  {!esCerrado && mes.proyeccion2026 != null ? (
                    editando[mes.proyeccion2026] !== undefined ? (
                      <div className="edicion-monto">
                        <input
                          type="number"
                          step="0.01"
                          value={editando[mes.proyeccion2026]}
                          onChange={(e) => setEditando({ ...editando, [mes.proyeccion2026]: e.target.value })}
                          className="input-monto"
                        />
                        <button 
                          onClick={() => handleGuardarMonto(mes.proyeccion2026)}
                          className="btn-guardar-mini"
                        >
                          ✓
                        </button>
                        <button 
                          onClick={() => handleCancelarEdicion(mes.proyeccion2026)}
                          className="btn-cancelar-mini"
                        >
                          ✗
                        </button>
                      </div>
                    ) : (
                      <div className="monto-editable" onClick={() => handleEditarMonto(mes.proyeccion2026, mes.proyeccion2026)}>
                        {formatearMoneda(mes.proyeccion2026)}
                        <span className="icono-editar">✎</span>
                      </div>
                    )
                  ) : (
                    formatearMoneda(mes.proyeccion2026)
                  )}
                </td>
                <td className={`porcentaje ${getPorcentajeClass(mes.porcentajeCrecimiento)}`}>
                  {formatearPorcentaje(mes.porcentajeCrecimiento)}
                </td>
              </tr>
            ))}
          </tbody>
          <tfoot>
            <tr className="total-row">
              <td><strong>Total</strong></td>
              <td className="monto"><strong>{formatearMoneda(resumen.meses.reduce((sum, m) => sum + (m.venta2024 || 0), 0))}</strong></td>
              <td className="monto"><strong>{formatearMoneda(resumen.meses.reduce((sum, m) => sum + (m.venta2025 || 0), 0))}</strong></td>
              <td className="monto"><strong>{formatearMoneda(resumen.totalProyectado)}</strong></td>
              <td className={`porcentaje ${getPorcentajeClass(resumen.porcentajeCrecimiento2025vs2026)}`}>
                <strong>{formatearPorcentaje(resumen.porcentajeCrecimiento2025vs2026)}</strong>
              </td>
            </tr>
            <tr className="crecimiento-row">
              <td colSpan="2"><strong>% Crecimiento 2024 vs 2025</strong></td>
              <td colSpan="3" className={`porcentaje ${getPorcentajeClass(resumen.porcentajeCrecimiento2024vs2025)}`}>
                <strong>{formatearPorcentaje(resumen.porcentajeCrecimiento2024vs2025)}</strong>
              </td>
            </tr>
            <tr className="crecimiento-row">
              <td colSpan="2"><strong>% Crecimiento 2025 vs 2026</strong></td>
              <td colSpan="3" className={`porcentaje ${getPorcentajeClass(resumen.porcentajeCrecimiento2025vs2026)}`}>
                <strong>{formatearPorcentaje(resumen.porcentajeCrecimiento2025vs2026)}</strong>
              </td>
            </tr>
          </tfoot>
        </table>
      </div>

      <div className="nota-campos">
        <p><span className="campo-verde">●</span> Campos Verdes: corresponden a selección.</p>
        <p><span className="campo-gris">●</span> Campos grises: corresponden a datos de visualización o calculados.</p>
        <p><span className="campo-celeste">●</span> Campos celestes: datos ingresados.</p>
      </div>
    </div>
  )
}

export default ProyeccionVentas

# Planificación del Sistema de Proyección de Ventas - Los Ladrillos S.A.

## 1. Resumen del Proyecto

Sistema web para gestionar y proyectar ventas mensuales del año 2026 para las tiendas de Los Ladrillos S.A. en Guatemala. El sistema permite ingresar proyecciones, calcular crecimientos y controlar el acceso mediante roles.

## 2. Arquitectura Técnica

### Stack Tecnológico
- **Frontend**: React 18 con TypeScript
- **Backend**: .NET 8 Web API (C#)
- **Base de Datos**: SQL Server 2022 (Docker)
- **Servidor Web**: Nginx (reverse proxy)
- **Containerización**: Docker + Docker Compose

### Arquitectura General
```
Cliente (Browser)
    ↓
Nginx:80 (Reverse Proxy)
    ↓
React App (Frontend)
    ↓
.NET API:5000 (Backend)
    ↓
SQL Server:1433 (Database)
```

## 3. Estructura del Proyecto

```
prueba-tecnica-desarrollador-SM/
├── backend/                    # API .NET
│   ├── Controllers/           # Endpoints REST
│   ├── Models/               # Entidades y DTOs
│   ├── Services/             # Lógica de negocio
│   ├── Data/                 # Contexto EF Core
│   └── Dockerfile
├── frontend/                  # Aplicación React
│   ├── src/
│   │   ├── components/       # Componentes UI
│   │   ├── services/         # Llamadas API
│   │   ├── hooks/           # Custom hooks
│   │   └── types/           # TypeScript types
│   └── Dockerfile
├── database/                  # Scripts SQL
│   ├── init.sql             # Creación de BD
│   ├── functions.sql        # Funciones de cálculo
│   └── data.sql             # Datos iniciales
├── nginx/                     # Configuración Nginx
│   └── nginx.conf
├── docker-compose.yml
└── docs/                      # Documentación
    ├── DATABASE.md
    ├── API.md
    └── DEPLOYMENT.md
```

## 4. Modelo de Datos

### Tablas Principales

**Paises**
- Almacena países donde opera la empresa
- Campos: id, codigo, nombre, moneda, activo

**Tiendas**
- Información de cada sucursal
- Campos: id, nombre, codigo, pais_id, activo

**Usuarios**
- Usuarios del sistema con roles
- Campos: id, username, password_hash, rol, activo

**VentasHistoricas**
- Ventas reales de años anteriores (2024-2025)
- Campos: id, tienda_id, año, mes, monto

**ProyeccionVentas**
- Proyecciones para 2026
- Campos: id, tienda_id, año, mes, monto_proyectado, estado, usuario_id

**BitacoraMovimientos**
- Registro de auditoría
- Campos: id, usuario_id, accion, tabla, registro_id, valores_anteriores, valores_nuevos, fecha

## 5. Funcionalidades Clave

### 5.1 Autenticación y Autorización
- Login con usuario y contraseña
- JWT para autenticación
- Rol de Gerente para acceso completo
- Middleware de autorización en API

### 5.2 Gestión de Proyecciones
- **Data Entry**: Formulario para ingresar proyecciones mes a mes por tienda
- **Visualización**: Tabla comparativa 2024, 2025 y proyección 2026
- **Cálculos Automáticos**: 
  - % Crecimiento 2024 vs 2025
  - % Crecimiento 2025 vs Proyección 2026
- **Estados**: ABIERTO (editable) / CERRADO (solo lectura)

### 5.3 Funciones de Base de Datos
```sql
-- Función 1: Calcular crecimiento 2024 vs 2025
CREATE FUNCTION fn_CrecimientoVentas2024vs2025(@tiendaId INT, @mes INT)
RETURNS DECIMAL(10,2)

-- Función 2: Calcular crecimiento proyección 2026 vs 2025
CREATE FUNCTION fn_CrecimientoProyeccion2026vs2025(@tiendaId INT, @mes INT)
RETURNS DECIMAL(10,2)
```

### 5.4 Bitácora de Auditoría
- Registro automático de todos los cambios
- Captura usuario, fecha, acción y valores modificados
- Almacenamiento en formato JSON para flexibilidad

### 5.5 Control de Estados
- Botón "Cerrar" para finalizar proyecciones
- Validación de estado antes de permitir ediciones
- Solo usuarios autorizados pueden cambiar estados

## 6. Flujo de Trabajo

1. **Login**: Usuario gerente inicia sesión
2. **Selección**: Elige año (2026) y tienda
3. **Ingreso**: Captura proyecciones mensuales
4. **Revisión**: Visualiza datos históricos y porcentajes de crecimiento
5. **Cierre**: Cuando termina, cierra el registro (estado CERRADO)
6. **Auditoría**: Sistema registra todos los movimientos en bitácora

## 7. Seguridad

- Contraseñas hasheadas con BCrypt
- Tokens JWT con expiración
- Validación de roles en cada endpoint
- CORS configurado correctamente
- SQL parametrizado para prevenir inyección
- Validación de entrada en frontend y backend

## 8. Deployment con Docker

### Servicios Docker
1. **sqlserver**: SQL Server 2022 con volúmenes persistentes
2. **backend**: API .NET publicada en modo Release
3. **frontend**: Build de React servido por Nginx
4. **nginx**: Reverse proxy principal

### Comandos de Deployment
```bash
# Iniciar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```

## 9. Escalabilidad Futura

El sistema está preparado para:
- ✅ Agregar más países (El Salvador, Honduras, Nicaragua, México)
- ✅ Múltiples años de proyección
- ✅ Más roles de usuario (Vendedor, Analista, Admin)
- ✅ Reportes y exportación a Excel
- ✅ Gráficas de tendencias
- ✅ APIs RESTful documentadas con Swagger

## 10. Cronograma de Desarrollo

| Fase | Tiempo Estimado |
|------|----------------|
| Setup de infraestructura (Docker) | 2 horas |
| Base de datos y funciones | 3 horas |
| Backend API | 5 horas |
| Frontend React | 6 horas |
| Integración y pruebas | 3 horas |
| Documentación | 2 horas |
| **Total** | **~21 horas** |

## 11. Criterios de Éxito

- ✅ Sistema completamente funcional
- ✅ Todos los requerimientos implementados
- ✅ Código limpio y bien estructurado
- ✅ Documentación completa
- ✅ Fácil de desplegar con Docker
- ✅ Interfaz intuitiva y profesional
- ✅ Seguridad implementada correctamente

---

**Nota**: Esta planificación prioriza funcionalidad, mantenibilidad y buenas prácticas de desarrollo, con especial énfasis en crear un sistema profesional y escalable.

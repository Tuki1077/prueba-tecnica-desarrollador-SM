# Los Ladrillos S.A. - Sistema de Proyección de Ventas
## Documentación Técnica Completa

---

## Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Tecnologías Utilizadas](#tecnologías-utilizadas)
4. [Estructura de la Base de Datos](#estructura-de-la-base-de-datos)
5. [API REST - Endpoints](#api-rest---endpoints)
6. [Instalación y Configuración](#instalación-y-configuración)
7. [Uso del Sistema](#uso-del-sistema)

---

## Descripción General

Sistema web para la gestión y proyección de ventas de Los Ladrillos S.A., permitiendo:

- **Data entry** para registrar proyecciones de ventas anuales por sucursal
- Proyecciones en moneda local (actualmente Guatemala - GTQ)
- Cálculo automático de % de crecimiento entre años
- Función de cierre para proteger proyecciones finalizadas
- Bitácora completa de movimientos
- Control de acceso basado en roles (GERENTE)

---

## Arquitectura del Sistema

### Diagrama de Arquitectura

```
┌─────────────┐      ┌──────────────┐      ┌───────────────┐
│   Cliente   │ ───> │    Nginx     │ ───> │   Frontend    │
│  (Browser)  │      │ Reverse Proxy│      │   React+Vite  │
└─────────────┘      └──────────────┘      └───────────────┘
                             │
                             ↓
                     ┌──────────────┐
                     │   Backend    │
                     │  .NET 8 API  │
                     └──────────────┘
                             │
                             ↓
                     ┌──────────────┐
                     │  SQL Server  │
                     │     2022     │
                     └──────────────┘
```

### Componentes

- **Frontend**: React 18 con Vite, estilado con CSS puro
- **Backend**: .NET 8 Web API con Entity Framework Core
- **Base de Datos**: SQL Server 2022 Express
- **Servidor Web**: Nginx como reverse proxy
- **Contenedores**: Docker y Docker Compose

---

## Tecnologías Utilizadas

### Backend
- **.NET 8.0** - Framework principal
- **Entity Framework Core 8.0** - ORM
- **ASP.NET Core** - Web API
- **JWT Bearer** - Autenticación
- **BCrypt.Net** - Hash de contraseñas
- **Swagger/OpenAPI** - Documentación automática

### Frontend
- **React 18.2** - Librería UI
- **Vite 5.0** - Build tool
- **React Router 6** - Navegación
- **Axios** - Cliente HTTP

### Base de Datos
- **SQL Server 2022** Express Edition
- **Funciones personalizadas** para cálculos de crecimiento

### DevOps
- **Docker & Docker Compose** - Contenedorización
- **Nginx** - Reverse proxy y servidor web

---

## Estructura de la Base de Datos

### Diagrama Entidad-Relación

```
┌───────────────┐       ┌──────────────┐       ┌─────────────────┐
│   Usuarios    │       │    Paises    │       │    Tiendas      │
├───────────────┤       ├──────────────┤       ├─────────────────┤
│ UsuarioId PK  │       │ PaisId PK    │       │ TiendaId PK     │
│ NombreUsuario │       │ CodigoPais   │  ┌───>│ CodigoTienda    │
│ PasswordHash  │       │ NombrePais   │  │    │ NombreTienda    │
│ Rol           │       │ CodigoMoneda │  │    │ PaisId FK       │
│ Activo        │       │ NombreMoneda │  │    │ Activa          │
│ FechaCreacion │       │ Activo       │  │    └─────────────────┘
└───────────────┘       └──────────────┘  │             │
        │                        │         │             │
        │                        └─────────┘             │
        │                                                │
        ↓                                                ↓
┌───────────────┐                           ┌───────────────────────┐
│   Bitacoras   │                           │  VentasHistoricas     │
├───────────────┤                           ├───────────────────────┤
│ BitacoraId PK │                           │ VentaHistoricaId PK   │
│ UsuarioId FK  │                           │ TiendaId FK           │
│ Accion        │                           │ Anio                  │
│ Tabla         │                           │ Mes                   │
│ RegistroId    │                           │ MontoVenta            │
│ ValoresAnt.   │                           │ FechaRegistro         │
│ ValoresNuevos │                           └───────────────────────┘
│ FechaHora     │                                        │
│ DireccionIP   │                                        │
└───────────────┘                                        │
        ↑                                                ↓
        │                           ┌───────────────────────────────┐
        │                           │   ProyeccionesVentas          │
        │                           ├───────────────────────────────┤
        │                           │ ProyeccionVentaId PK          │
        │                           │ TiendaId FK                   │
        │                           │ Anio                          │
        │                           │ Mes                           │
        │                           │ MontoProyectado               │
        │                           │ Estado (ABIERTO/CERRADO)      │
        │                           │ UsuarioRegistroId FK          │
        │                           │ FechaRegistro                 │
        └───────────────────────────│ UsuarioModificacionId FK      │
                                    │ FechaModificacion             │
                                    │ UsuarioCierreId FK            │
                                    │ FechaCierre                   │
                                    └───────────────────────────────┘
```

### Definición de Tablas

#### Usuarios
```sql
CREATE TABLE Usuarios (
    UsuarioId INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Rol NVARCHAR(20) NOT NULL DEFAULT 'GERENTE',
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE()
)
```

#### Paises
```sql
CREATE TABLE Paises (
    PaisId INT PRIMARY KEY IDENTITY(1,1),
    CodigoPais NVARCHAR(3) NOT NULL,
    NombrePais NVARCHAR(100) NOT NULL,
    CodigoMoneda NVARCHAR(10) NOT NULL,
    NombreMoneda NVARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
)
```

#### Tiendas
```sql
CREATE TABLE Tiendas (
    TiendaId INT PRIMARY KEY IDENTITY(1,1),
    CodigoTienda NVARCHAR(10) NOT NULL,
    NombreTienda NVARCHAR(100) NOT NULL,
    PaisId INT NOT NULL,
    Activa BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (PaisId) REFERENCES Paises(PaisId),
    UNIQUE (CodigoTienda, PaisId)
)
```

#### VentasHistoricas
```sql
CREATE TABLE VentasHistoricas (
    VentaHistoricaId INT PRIMARY KEY IDENTITY(1,1),
    TiendaId INT NOT NULL,
    Anio INT NOT NULL,
    Mes INT NOT NULL,
    MontoVenta DECIMAL(18,2) NOT NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (TiendaId) REFERENCES Tiendas(TiendaId),
    UNIQUE (TiendaId, Anio, Mes)
)
```

#### ProyeccionesVentas
```sql
CREATE TABLE ProyeccionesVentas (
    ProyeccionVentaId INT PRIMARY KEY IDENTITY(1,1),
    TiendaId INT NOT NULL,
    Anio INT NOT NULL,
    Mes INT NOT NULL,
    MontoProyectado DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'ABIERTO',
    UsuarioRegistroId INT NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
    UsuarioModificacionId INT NULL,
    FechaModificacion DATETIME2 NULL,
    UsuarioCierreId INT NULL,
    FechaCierre DATETIME2 NULL,
    FOREIGN KEY (TiendaId) REFERENCES Tiendas(TiendaId),
    FOREIGN KEY (UsuarioRegistroId) REFERENCES Usuarios(UsuarioId),
    FOREIGN KEY (UsuarioModificacionId) REFERENCES Usuarios(UsuarioId),
    FOREIGN KEY (UsuarioCierreId) REFERENCES Usuarios(UsuarioId),
    UNIQUE (TiendaId, Anio, Mes)
)
```

#### Bitacoras
```sql
CREATE TABLE Bitacoras (
    BitacoraId INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    Accion NVARCHAR(50) NOT NULL,
    Tabla NVARCHAR(100) NOT NULL,
    RegistroId INT NULL,
    ValoresAnteriores NVARCHAR(1000) NULL,
    ValoresNuevos NVARCHAR(1000) NULL,
    FechaHora DATETIME2 NOT NULL DEFAULT GETDATE(),
    DireccionIP NVARCHAR(50) NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId)
)
```

### Funciones SQL Personalizadas

#### Función 1: Crecimiento 2024 vs 2025
```sql
CREATE FUNCTION dbo.fn_CrecimientoVentas2024vs2025 (
    @tiendaId INT,
    @mes INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @ventas2024 DECIMAL(18,2);
    DECLARE @ventas2025 DECIMAL(18,2);
    DECLARE @crecimiento DECIMAL(10,2);

    SELECT @ventas2024 = MontoVenta FROM VentasHistoricas 
    WHERE TiendaId = @tiendaId AND Anio = 2024 AND Mes = @mes;

    SELECT @ventas2025 = MontoVenta FROM VentasHistoricas 
    WHERE TiendaId = @tiendaId AND Anio = 2025 AND Mes = @mes;

    IF @ventas2024 IS NULL OR @ventas2024 = 0
        SET @crecimiento = 0;
    ELSE IF @ventas2025 IS NULL
        SET @crecimiento = 0;
    ELSE
        SET @crecimiento = ((@ventas2025 - @ventas2024) / @ventas2024) * 100;

    RETURN @crecimiento;
END
```

#### Función 2: Crecimiento 2025 vs 2026
```sql
CREATE FUNCTION dbo.fn_CrecimientoProyeccion2026vs2025 (
    @tiendaId INT,
    @mes INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @ventas2025 DECIMAL(18,2);
    DECLARE @proyeccion2026 DECIMAL(18,2);
    DECLARE @crecimiento DECIMAL(10,2);

    SELECT @ventas2025 = MontoVenta FROM VentasHistoricas 
    WHERE TiendaId = @tiendaId AND Anio = 2025 AND Mes = @mes;

    SELECT @proyeccion2026 = MontoProyectado FROM ProyeccionesVentas 
    WHERE TiendaId = @tiendaId AND Anio = 2026 AND Mes = @mes;

    IF @ventas2025 IS NULL OR @ventas2025 = 0
        SET @crecimiento = 0;
    ELSE IF @proyeccion2026 IS NULL
        SET @crecimiento = 0;
    ELSE
        SET @crecimiento = ((@proyeccion2026 - @ventas2025) / @ventas2025) * 100;

    RETURN @crecimiento;
END
```

---

## API REST - Endpoints

Base URL: `http://localhost/api`

### Autenticación

#### POST /api/auth/login
Iniciar sesión en el sistema.

**Request Body:**
```json
{
  "nombreUsuario": "gerente",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "nombreUsuario": "gerente",
  "rol": "GERENTE",
  "usuarioId": 1
}
```

### Tiendas

#### GET /api/tiendas
Obtener todas las tiendas activas.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
[
  {
    "tiendaId": 1,
    "codigoTienda": "10",
    "nombreTienda": "Ciudad",
    "paisId": 1,
    "pais": {
      "paisId": 1,
      "codigoPais": "GT",
      "nombrePais": "Guatemala",
      "codigoMoneda": "GTQ",
      "nombreMoneda": "Quetzal"
    }
  }
]
```

#### GET /api/tiendas/pais/{codigoPais}
Obtener tiendas por código de país.

**Headers:** `Authorization: Bearer {token}`

### Proyecciones

#### GET /api/proyecciones/{anio}/{tiendaId}
Obtener resumen completo de proyección.

**Headers:** `Authorization: Bearer {token}`

**Response (200 OK):**
```json
{
  "anio": 2026,
  "tiendaId": 1,
  "codigoTienda": "10",
  "nombreTienda": "Ciudad",
  "moneda": "GTQ",
  "estado": "ABIERTO",
  "meses": [
    {
      "mes": 1,
      "nombreMes": "Enero",
      "venta2024": 350000.00,
      "venta2025": 260000.00,
      "proyeccion2026": 350000.00,
      "porcentajeCrecimiento": 34.62
    }
  ],
  "totalProyectado": 4620000.00,
  "porcentajeCrecimiento2024vs2025": 15.00,
  "porcentajeCrecimiento2025vs2026": 25.00
}
```

#### POST /api/proyecciones
Crear nueva proyección de venta.

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "tiendaId": 1,
  "anio": 2026,
  "mes": 1,
  "montoProyectado": 350000.00
}
```

#### PUT /api/proyecciones
Actualizar proyección existente (solo si está ABIERTA).

**Headers:** `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "proyeccionVentaId": 1,
  "montoProyectado": 375000.00
}
```

#### POST /api/proyecciones/cerrar/{anio}/{tiendaId}
Cerrar proyecciones de un año específico.

**Headers:** `Authorization: Bearer {token}`

### Bitácora

#### GET /api/bitacora
Obtener registros de bit

ácora con filtros opcionales.

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**
- `fechaInicio` (optional): Fecha inicio en formato ISO
- `fechaFin` (optional): Fecha fin en formato ISO
- `usuarioId` (optional): ID del usuario
- `accion` (optional): Tipo de acción (LOGIN, INSERT, UPDATE, DELETE, CIERRE)

---

## Instalación y Configuración

### Requisitos Previos

- **Docker** >= 20.10
- **Docker Compose** >= 2.0
- **Git**

### Pasos de Instalación

1. **Clonar el repositorio:**
```bash
git clone https://github.com/Tuki1077/prueba-tecnica-desarrollador-SM.git
cd prueba-tecnica-desarrollador-SM
```

2. **Iniciar todos los servicios con Docker Compose:**
```bash
docker-compose up -d
```

Esto levantará:
- SQL Server (puerto 1433)
- Backend API (puerto 5000)
- Frontend React (puerto 3000)
- Nginx (puerto 80)

3. **Verificar que los servicios estén corriendo:**
```bash
docker-compose ps
```

4. **Acceder a la aplicación:**
- **Frontend**: http://localhost
- **API**: http://localhost/api
- **Swagger**: http://localhost/api (redirige automáticamente)

### Credenciales Iniciales

El sistema crea un usuario por defecto:
- **Usuario**: `gerente`
- **Contraseña**: `password123`
- **Rol**: GERENTE

### Datos Iniciales

La base de datos se inicializa con:
- País: Guatemala (GT)
- 3 Tiendas: Ciudad (10), Petén (20), Xela (30)
- Ventas históricas 2024 y 2025 para todas las tiendas

---

## Uso del Sistema

### 1. Inicio de Sesión

Accede a http://localhost e ingresa con las credenciales de gerente.

### 2. Pantalla Principal - Proyección de Ventas

- **Seleccionar Año**: 2026 (único disponible actualmente)
- **Seleccionar Tienda**: Ciudad, Petén o Xela
- **Ver Datos Históricos**: Ventas 2024 y 2025 precargadas
- **Ingresar Proyecciones**: Click en los montos de proyección 2026 para editarlos
- **Guardar Cambios**: Click en ✓ para guardar, ✗ para cancelar
- **Ver % Crecimiento**: Calculado automáticamente

### 3. Cerrar Proyección

Una vez completada la proyección, click en **"Cerrar"**. Esto:
- Cambia el estado a CERRADO
- No permite más modificaciones
- Registra la acción en bitácora

### 4. Bitácora de Movimientos

Click en **"Ver Bitácora"** para acceder al historial completo de:
- Inicios de sesión
- Creación de proyecciones
- Modificaciones
- Cierres

Filtros disponibles:
- Rango de fechas
- Tipo de acción
- Usuario

---

## Características de Seguridad

- **Autenticación JWT**: Tokens con expiración de 8 horas
- **Contraseñas hasheadas**: BCrypt con salt automático
- **Autorización por roles**: Solo usuarios GERENTE pueden acceder
- **Auditoría completa**: Toda acción se registra en bitácora con IP
- **Validación de estado**: No se pueden modificar proyecciones cerradas
- **HTTPS ready**: Configuración preparada para certificados SSL

---

## Mantenimiento

### Ver logs de los servicios:
```bash
docker-compose logs -f [servicio]
```

### Reiniciar un servicio:
```bash
docker-compose restart [servicio]
```

### Detener todos los servicios:
```bash
docker-compose down
```

### Backup de la base de datos:
```bash
docker exec losladrillos-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'LosLadrillos2024!' \
  -Q "BACKUP DATABASE VentasProyeccion TO DISK='/var/opt/mssql/data/VentasProyeccion.bak'"
```

---

## Soporte y Contacto

Para dudas o problemas técnicos, contactar al equipo de desarrollo de Los Ladrillos S.A.

---

**Versión**: 1.0.0  
**Fecha**: Noviembre 2025  
**Desarrollado para**: Los Ladrillos S.A.

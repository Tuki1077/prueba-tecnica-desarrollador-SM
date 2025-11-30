-- ==================================================
-- Sistema de Proyección de Ventas - Los Ladrillos S.A.
-- Base de Datos: VentasProyeccion
-- ==================================================

USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'VentasProyeccion')
BEGIN
    CREATE DATABASE VentasProyeccion;
END
GO

USE VentasProyeccion;
GO

-- ==================================================
-- TABLA: Paises
-- Descripción: Almacena los países donde opera la empresa
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Paises')
BEGIN
    CREATE TABLE Paises (
        id INT PRIMARY KEY IDENTITY(1,1),
        codigo VARCHAR(5) NOT NULL UNIQUE,
        nombre VARCHAR(100) NOT NULL,
        moneda VARCHAR(50) NOT NULL,
        activo BIT DEFAULT 1,
        fecha_creacion DATETIME DEFAULT GETDATE(),
        fecha_modificacion DATETIME DEFAULT GETDATE()
    );
END
GO

-- ==================================================
-- TABLA: Tiendas
-- Descripción: Información de cada sucursal por país
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tiendas')
BEGIN
    CREATE TABLE Tiendas (
        id INT PRIMARY KEY IDENTITY(1,1),
        nombre VARCHAR(100) NOT NULL,
        codigo VARCHAR(20) NOT NULL UNIQUE,
        pais_id INT NOT NULL,
        activo BIT DEFAULT 1,
        fecha_creacion DATETIME DEFAULT GETDATE(),
        fecha_modificacion DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Tiendas_Paises FOREIGN KEY (pais_id) REFERENCES Paises(id)
    );
END
GO

-- ==================================================
-- TABLA: Usuarios
-- Descripción: Usuarios del sistema con roles
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        id INT PRIMARY KEY IDENTITY(1,1),
        username VARCHAR(50) NOT NULL UNIQUE,
        password_hash VARCHAR(255) NOT NULL,
        nombre_completo VARCHAR(150) NOT NULL,
        rol VARCHAR(20) NOT NULL CHECK (rol IN ('GERENTE', 'ANALISTA', 'ADMIN')),
        activo BIT DEFAULT 1,
        fecha_creacion DATETIME DEFAULT GETDATE(),
        fecha_modificacion DATETIME DEFAULT GETDATE()
    );
END
GO

-- ==================================================
-- TABLA: VentasHistoricas
-- Descripción: Ventas reales de años anteriores (2024-2025)
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VentasHistoricas')
BEGIN
    CREATE TABLE VentasHistoricas (
        id INT PRIMARY KEY IDENTITY(1,1),
        tienda_id INT NOT NULL,
        anio INT NOT NULL CHECK (anio >= 2020 AND anio <= 2030),
        mes INT NOT NULL CHECK (mes >= 1 AND mes <= 12),
        monto DECIMAL(18,2) NOT NULL CHECK (monto >= 0),
        fecha_creacion DATETIME DEFAULT GETDATE(),
        fecha_modificacion DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_VentasHistoricas_Tiendas FOREIGN KEY (tienda_id) REFERENCES Tiendas(id),
        CONSTRAINT UQ_VentasHistoricas UNIQUE (tienda_id, anio, mes)
    );
END
GO

-- ==================================================
-- TABLA: ProyeccionVentas
-- Descripción: Proyecciones de ventas futuras (2026+)
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProyeccionVentas')
BEGIN
    CREATE TABLE ProyeccionVentas (
        id INT PRIMARY KEY IDENTITY(1,1),
        tienda_id INT NOT NULL,
        anio INT NOT NULL CHECK (anio >= 2026),
        mes INT NOT NULL CHECK (mes >= 1 AND mes <= 12),
        monto_proyectado DECIMAL(18,2) NOT NULL CHECK (monto_proyectado >= 0),
        estado VARCHAR(20) NOT NULL DEFAULT 'ABIERTO' CHECK (estado IN ('ABIERTO', 'CERRADO')),
        usuario_creacion_id INT NOT NULL,
        usuario_modificacion_id INT NULL,
        fecha_creacion DATETIME DEFAULT GETDATE(),
        fecha_modificacion DATETIME DEFAULT GETDATE(),
        fecha_cierre DATETIME NULL,
        CONSTRAINT FK_ProyeccionVentas_Tiendas FOREIGN KEY (tienda_id) REFERENCES Tiendas(id),
        CONSTRAINT FK_ProyeccionVentas_Usuario_Creacion FOREIGN KEY (usuario_creacion_id) REFERENCES Usuarios(id),
        CONSTRAINT FK_ProyeccionVentas_Usuario_Modificacion FOREIGN KEY (usuario_modificacion_id) REFERENCES Usuarios(id),
        CONSTRAINT UQ_ProyeccionVentas UNIQUE (tienda_id, anio, mes)
    );
END
GO

-- ==================================================
-- TABLA: BitacoraMovimientos
-- Descripción: Registro de auditoría de todos los cambios
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BitacoraMovimientos')
BEGIN
    CREATE TABLE BitacoraMovimientos (
        id INT PRIMARY KEY IDENTITY(1,1),
        usuario_id INT NOT NULL,
        accion VARCHAR(50) NOT NULL CHECK (accion IN ('INSERT', 'UPDATE', 'DELETE', 'CERRAR', 'ABRIR', 'LOGIN')),
        tabla VARCHAR(50) NOT NULL,
        registro_id INT NOT NULL,
        valores_anteriores NVARCHAR(MAX) NULL,
        valores_nuevos NVARCHAR(MAX) NULL,
        ip_address VARCHAR(45) NULL,
        fecha_accion DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_BitacoraMovimientos_Usuarios FOREIGN KEY (usuario_id) REFERENCES Usuarios(id)
    );
END
GO

-- ==================================================
-- ÍNDICES para optimizar consultas
-- ==================================================

-- Índice para búsquedas por tienda y año en VentasHistoricas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VentasHistoricas_Tienda_Anio')
BEGIN
    CREATE INDEX IX_VentasHistoricas_Tienda_Anio ON VentasHistoricas(tienda_id, anio);
END
GO

-- Índice para búsquedas por tienda y año en ProyeccionVentas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProyeccionVentas_Tienda_Anio')
BEGIN
    CREATE INDEX IX_ProyeccionVentas_Tienda_Anio ON ProyeccionVentas(tienda_id, anio);
END
GO

-- Índice para búsquedas por estado en ProyeccionVentas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProyeccionVentas_Estado')
BEGIN
    CREATE INDEX IX_ProyeccionVentas_Estado ON ProyeccionVentas(estado);
END
GO

-- Índice para búsquedas en BitacoraMovimientos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_BitacoraMovimientos_Usuario_Fecha')
BEGIN
    CREATE INDEX IX_BitacoraMovimientos_Usuario_Fecha ON BitacoraMovimientos(usuario_id, fecha_accion DESC);
END
GO

PRINT 'Base de datos y tablas creadas exitosamente';
GO

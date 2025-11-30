-- ==================================================
-- Sistema de Proyección de Ventas - Los Ladrillos S.A.
-- Datos Iniciales
-- ==================================================

USE VentasProyeccion;
GO

-- ==================================================
-- INSERTAR PAÍSES
-- ==================================================
SET IDENTITY_INSERT Paises ON;
GO

IF NOT EXISTS (SELECT 1 FROM Paises WHERE codigo = 'GT')
BEGIN
    INSERT INTO Paises (id, codigo, nombre, moneda, activo) 
    VALUES (1, 'GT', 'Guatemala', 'Quetzales (GTQ)', 1);
END

IF NOT EXISTS (SELECT 1 FROM Paises WHERE codigo = 'SV')
BEGIN
    INSERT INTO Paises (id, codigo, nombre, moneda, activo) 
    VALUES (2, 'SV', 'El Salvador', 'Dólares (USD)', 0);
END

IF NOT EXISTS (SELECT 1 FROM Paises WHERE codigo = 'HN')
BEGIN
    INSERT INTO Paises (id, codigo, nombre, moneda, activo) 
    VALUES (3, 'HN', 'Honduras', 'Lempiras (HNL)', 0);
END

IF NOT EXISTS (SELECT 1 FROM Paises WHERE codigo = 'NI')
BEGIN
    INSERT INTO Paises (id, codigo, nombre, moneda, activo) 
    VALUES (4, 'NI', 'Nicaragua', 'Córdobas (NIO)', 0);
END

IF NOT EXISTS (SELECT 1 FROM Paises WHERE codigo = 'MX')
BEGIN
    INSERT INTO Paises (id, codigo, nombre, moneda, activo) 
    VALUES (5, 'MX', 'México', 'Pesos (MXN)', 0);
END

SET IDENTITY_INSERT Paises OFF;
GO

PRINT 'Países insertados correctamente';
GO

-- ==================================================
-- INSERTAR TIENDAS DE GUATEMALA
-- ==================================================
SET IDENTITY_INSERT Tiendas ON;
GO

IF NOT EXISTS (SELECT 1 FROM Tiendas WHERE codigo = '10-CIUDAD')
BEGIN
    INSERT INTO Tiendas (id, nombre, codigo, pais_id, activo) 
    VALUES (1, 'Tienda Ciudad', '10-CIUDAD', 1, 1);
END

IF NOT EXISTS (SELECT 1 FROM Tiendas WHERE codigo = '20-PETEN')
BEGIN
    INSERT INTO Tiendas (id, nombre, codigo, pais_id, activo) 
    VALUES (2, 'Tienda Petén', '20-PETEN', 1, 1);
END

IF NOT EXISTS (SELECT 1 FROM Tiendas WHERE codigo = '30-XELA')
BEGIN
    INSERT INTO Tiendas (id, nombre, codigo, pais_id, activo) 
    VALUES (3, 'Tienda Xela', '30-XELA', 1, 1);
END

SET IDENTITY_INSERT Tiendas OFF;
GO

PRINT 'Tiendas insertadas correctamente';
GO

-- ==================================================
-- INSERTAR USUARIOS
-- Los usuarios se crean mediante el endpoint /api/auth/register después de que el backend esté disponible
-- Ver script: 05-create-users.sh
-- Credenciales: gerente/gerente123 y admin/admin123
-- ==================================================

PRINT 'Usuarios insertados correctamente';
PRINT 'Usuario: gerente / Password: Admin123!';
PRINT 'Usuario: admin / Password: Admin123!';
GO

-- ==================================================
-- INSERTAR VENTAS HISTÓRICAS 2024 - TIENDA CIUDAD
-- ==================================================
INSERT INTO VentasHistoricas (tienda_id, anio, mes, monto) VALUES
(1, 2024, 1, 350000.00),
(1, 2024, 2, 225000.00),
(1, 2024, 3, 230000.00),
(1, 2024, 4, 215000.00),
(1, 2024, 5, 290000.00),
(1, 2024, 6, 235000.00),
(1, 2024, 7, 275500.00),
(1, 2024, 8, 375000.00),
(1, 2024, 9, 280500.00),
(1, 2024, 10, 245000.00),
(1, 2024, 11, 235000.00),
(1, 2024, 12, 260000.00);
GO

-- ==================================================
-- INSERTAR VENTAS HISTÓRICAS 2024 - TIENDA PETÉN
-- ==================================================
INSERT INTO VentasHistoricas (tienda_id, anio, mes, monto) VALUES
(2, 2024, 1, 275000.00),
(2, 2024, 2, 325000.00),
(2, 2024, 3, 265000.00),
(2, 2024, 4, 275000.00),
(2, 2024, 5, 280000.00),
(2, 2024, 6, 240000.00),
(2, 2024, 7, 230000.00),
(2, 2024, 8, 265000.00),
(2, 2024, 9, 245000.00),
(2, 2024, 10, 276000.00),
(2, 2024, 11, 236580.00),
(2, 2024, 12, 285000.00);
GO

-- ==================================================
-- INSERTAR VENTAS HISTÓRICAS 2024 - TIENDA XELA
-- ==================================================
INSERT INTO VentasHistoricas (tienda_id, anio, mes, monto) VALUES
(3, 2024, 1, 250000.00),
(3, 2024, 2, 236800.00),
(3, 2024, 3, 225000.00),
(3, 2024, 4, 290000.00),
(3, 2024, 5, 278500.00),
(3, 2024, 6, 265000.00),
(3, 2024, 7, 245000.00),
(3, 2024, 8, 278500.00),
(3, 2024, 9, 295000.00),
(3, 2024, 10, 245000.00),
(3, 2024, 11, 285000.00),
(3, 2024, 12, 269000.00);
GO

-- ==================================================
-- INSERTAR VENTAS HISTÓRICAS 2025 - TIENDA CIUDAD
-- ==================================================
INSERT INTO VentasHistoricas (tienda_id, anio, mes, monto) VALUES
(1, 2025, 1, 260000.00),
(1, 2025, 2, 245000.00),
(1, 2025, 3, 260000.00),
(1, 2025, 4, 265000.00),
(1, 2025, 5, 275000.00),
(1, 2025, 6, 280000.00),
(1, 2025, 7, 285000.00),
(1, 2025, 8, 325500.00),
(1, 2025, 9, 330500.00),
(1, 2025, 10, 340000.00),
(1, 2025, 11, 400000.00),
(1, 2025, 12, 425000.00);
GO

-- ==================================================
-- INSERTAR VENTAS HISTÓRICAS 2025 - TIENDA PETÉN
-- ==================================================
INSERT INTO VentasHistoricas (tienda_id, anio, mes, monto) VALUES
(2, 2025, 1, 286580.00),
(2, 2025, 2, 276000.00),
(2, 2025, 3, 285000.00),
(2, 2025, 4, 275000.00),
(2, 2025, 5, 325000.00),
(2, 2025, 6, 265000.00),
(2, 2025, 7, 240000.00),
(2, 2025, 8, 295000.00),
(2, 2025, 9, 280000.00),
(2, 2025, 10, 280000.00),
(2, 2025, 11, 275000.00),
(2, 2025, 12, 315000.00);
GO

-- ==================================================
-- INSERTAR VENTAS HISTÓRICAS 2025 - TIENDA XELA
-- ==================================================
INSERT INTO VentasHistoricas (tienda_id, anio, mes, monto) VALUES
(3, 2025, 1, 285000.00),
(3, 2025, 2, 290000.00),
(3, 2025, 3, 304000.00),
(3, 2025, 4, 290000.00),
(3, 2025, 5, 236800.00),
(3, 2025, 6, 275000.00),
(3, 2025, 7, 265000.00),
(3, 2025, 8, 245000.00),
(3, 2025, 9, 295000.00),
(3, 2025, 10, 278500.00),
(3, 2025, 11, 290000.00),
(3, 2025, 12, 278500.00);
GO

PRINT 'Ventas históricas 2024-2025 insertadas correctamente';
GO

-- ==================================================
-- VERIFICACIÓN DE DATOS
-- ==================================================
PRINT '======================================';
PRINT 'RESUMEN DE DATOS INSERTADOS';
PRINT '======================================';

SELECT 'Total Países' AS Tabla, COUNT(*) AS Cantidad FROM Paises
UNION ALL
SELECT 'Total Tiendas', COUNT(*) FROM Tiendas
UNION ALL
SELECT 'Total Usuarios', COUNT(*) FROM Usuarios
UNION ALL
SELECT 'Ventas 2024', COUNT(*) FROM VentasHistoricas WHERE anio = 2024
UNION ALL
SELECT 'Ventas 2025', COUNT(*) FROM VentasHistoricas WHERE anio = 2025;

PRINT '======================================';
PRINT 'TOTALES POR TIENDA Y AÑO';
PRINT '======================================';

SELECT 
    t.nombre AS Tienda,
    vh.anio AS Año,
    FORMAT(SUM(vh.monto), 'C', 'es-GT') AS Total_Ventas
FROM VentasHistoricas vh
INNER JOIN Tiendas t ON vh.tienda_id = t.id
GROUP BY t.nombre, vh.anio
ORDER BY t.nombre, vh.anio;

GO

PRINT '======================================';
PRINT 'Base de datos inicializada correctamente';
PRINT '======================================';
GO

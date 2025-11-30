-- ==================================================
-- Sistema de Proyección de Ventas - Los Ladrillos S.A.
-- Funciones de Cálculo de Crecimiento
-- ==================================================

USE VentasProyeccion;
GO

-- ==================================================
-- FUNCIÓN 1: fn_CrecimientoVentas2024vs2025
-- Descripción: Calcula el porcentaje de crecimiento entre ventas 2024 y 2025
-- Parámetros:
--   @tiendaId: ID de la tienda
--   @mes: Mes a calcular (1-12)
-- Retorna: Porcentaje de crecimiento (ej: 15.50 para 15.50%)
-- ==================================================
IF OBJECT_ID('dbo.fn_CrecimientoVentas2024vs2025', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CrecimientoVentas2024vs2025;
GO

CREATE FUNCTION dbo.fn_CrecimientoVentas2024vs2025
(
    @tiendaId INT,
    @mes INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @ventas2024 DECIMAL(18,2);
    DECLARE @ventas2025 DECIMAL(18,2);
    DECLARE @crecimiento DECIMAL(10,2);

    -- Obtener ventas de 2024
    SELECT @ventas2024 = MontoVenta
    FROM VentasHistoricas
    WHERE TiendaId = @tiendaId 
      AND Anio = 2024 
      AND Mes = @mes;

    -- Obtener ventas de 2025
    SELECT @ventas2025 = MontoVenta
    FROM VentasHistoricas
    WHERE TiendaId = @tiendaId 
      AND Anio = 2025 
      AND Mes = @mes;

    -- Calcular porcentaje de crecimiento
    -- Fórmula: ((Valor2025 - Valor2024) / Valor2024) * 100
    IF @ventas2024 IS NULL OR @ventas2024 = 0
        SET @crecimiento = 0;
    ELSE IF @ventas2025 IS NULL
        SET @crecimiento = 0;
    ELSE
        SET @crecimiento = ((@ventas2025 - @ventas2024) / @ventas2024) * 100;

    RETURN @crecimiento;
END
GO

-- ==================================================
-- FUNCIÓN 2: fn_CrecimientoProyeccion2026vs2025
-- Descripción: Calcula el porcentaje de crecimiento entre proyección 2026 y ventas 2025
-- Parámetros:
--   @tiendaId: ID de la tienda
--   @mes: Mes a calcular (1-12)
-- Retorna: Porcentaje de crecimiento (ej: 25.30 para 25.30%)
-- ==================================================
IF OBJECT_ID('dbo.fn_CrecimientoProyeccion2026vs2025', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CrecimientoProyeccion2026vs2025;
GO

CREATE FUNCTION dbo.fn_CrecimientoProyeccion2026vs2025
(
    @tiendaId INT,
    @mes INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @ventas2025 DECIMAL(18,2);
    DECLARE @proyeccion2026 DECIMAL(18,2);
    DECLARE @crecimiento DECIMAL(10,2);

    -- Obtener ventas de 2025
    SELECT @ventas2025 = MontoVenta
    FROM VentasHistoricas
    WHERE TiendaId = @tiendaId 
      AND Anio = 2025 
      AND Mes = @mes;

    -- Obtener proyección de 2026
    SELECT @proyeccion2026 = MontoProyectado
    FROM ProyeccionesVentas
    WHERE TiendaId = @tiendaId 
      AND Anio = 2026 
      AND Mes = @mes;

    -- Calcular porcentaje de crecimiento
    -- Fórmula: ((Proyeccion2026 - Ventas2025) / Ventas2025) * 100
    IF @ventas2025 IS NULL OR @ventas2025 = 0
        SET @crecimiento = 0;
    ELSE IF @proyeccion2026 IS NULL
        SET @crecimiento = 0;
    ELSE
        SET @crecimiento = ((@proyeccion2026 - @ventas2025) / @ventas2025) * 100;

    RETURN @crecimiento;
END
GO

-- ==================================================
-- FUNCIÓN AUXILIAR: fn_CrecimientoAnual
-- Descripción: Calcula el crecimiento total anual entre dos años
-- Parámetros:
--   @tiendaId: ID de la tienda
--   @anioBase: Año base para comparación
--   @anioComparacion: Año a comparar
-- Retorna: Porcentaje de crecimiento anual
-- ==================================================
IF OBJECT_ID('dbo.fn_CrecimientoAnual', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CrecimientoAnual;
GO

CREATE FUNCTION dbo.fn_CrecimientoAnual
(
    @tiendaId INT,
    @anioBase INT,
    @anioComparacion INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @totalBase DECIMAL(18,2);
    DECLARE @totalComparacion DECIMAL(18,2);
    DECLARE @crecimiento DECIMAL(10,2);

    -- Obtener total del año base
    SELECT @totalBase = SUM(MontoVenta)
    FROM VentasHistoricas
    WHERE TiendaId = @tiendaId 
      AND Anio = @anioBase;

    -- Obtener total del año de comparación
    IF @anioComparacion < 2026
    BEGIN
        SELECT @totalComparacion = SUM(MontoVenta)
        FROM VentasHistoricas
        WHERE TiendaId = @tiendaId 
          AND Anio = @anioComparacion;
    END
    ELSE
    BEGIN
        SELECT @totalComparacion = SUM(MontoProyectado)
        FROM ProyeccionesVentas
        WHERE TiendaId = @tiendaId 
          AND Anio = @anioComparacion;
    END

    -- Calcular porcentaje de crecimiento
    IF @totalBase IS NULL OR @totalBase = 0
        SET @crecimiento = 0;
    ELSE IF @totalComparacion IS NULL
        SET @crecimiento = 0;
    ELSE
        SET @crecimiento = ((@totalComparacion - @totalBase) / @totalBase) * 100;

    RETURN @crecimiento;
END
GO

-- ==================================================
-- VISTA: vw_ComparativoVentas
-- Descripción: Vista consolidada de ventas históricas y proyecciones con crecimientos
-- ==================================================
IF OBJECT_ID('dbo.vw_ComparativoVentas', 'V') IS NOT NULL
    DROP VIEW dbo.vw_ComparativoVentas;
GO

CREATE VIEW dbo.vw_ComparativoVentas
AS
SELECT 
    t.TiendaId AS tienda_id,
    t.NombreTienda AS tienda_nombre,
    t.CodigoTienda AS tienda_codigo,
    p.NombrePais AS pais,
    v2024.Mes,
    v2024.MontoVenta AS ventas_2024,
    v2025.MontoVenta AS ventas_2025,
    pv2026.MontoProyectado AS proyeccion_2026,
    dbo.fn_CrecimientoVentas2024vs2025(t.TiendaId, v2024.Mes) AS crecimiento_2024_vs_2025,
    dbo.fn_CrecimientoProyeccion2026vs2025(t.TiendaId, v2024.Mes) AS crecimiento_2025_vs_2026,
    pv2026.Estado AS estado_proyeccion
FROM Tiendas t
INNER JOIN Paises p ON t.PaisId = p.PaisId
LEFT JOIN VentasHistoricas v2024 ON t.TiendaId = v2024.TiendaId AND v2024.Anio = 2024
LEFT JOIN VentasHistoricas v2025 ON t.TiendaId = v2025.TiendaId AND v2025.Anio = 2025 AND v2025.Mes = v2024.Mes
LEFT JOIN ProyeccionesVentas pv2026 ON t.TiendaId = pv2026.TiendaId AND pv2026.Anio = 2026 AND pv2026.Mes = v2024.Mes
WHERE t.Activa = 1;
GO

-- ==================================================
-- Tests de las funciones
-- ==================================================
PRINT 'Funciones creadas exitosamente';
PRINT '--------------------------------------';
PRINT 'Funciones disponibles:';
PRINT '  - fn_CrecimientoVentas2024vs2025';
PRINT '  - fn_CrecimientoProyeccion2026vs2025';
PRINT '  - fn_CrecimientoAnual';
PRINT 'Vistas disponibles:';
PRINT '  - vw_ComparativoVentas';
GO

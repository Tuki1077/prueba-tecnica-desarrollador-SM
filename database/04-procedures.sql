-- ==================================================
-- Sistema de Proyección de Ventas - Los Ladrillos S.A.
-- Stored Procedures
-- ==================================================

USE VentasProyeccion;
GO

-- ==================================================
-- SP: sp_RegistrarBitacora
-- Descripción: Registra movimientos en la bitácora de auditoría
-- ==================================================
IF OBJECT_ID('dbo.sp_RegistrarBitacora', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarBitacora;
GO

CREATE PROCEDURE dbo.sp_RegistrarBitacora
    @usuario_id INT,
    @accion VARCHAR(50),
    @tabla VARCHAR(50),
    @registro_id INT,
    @valores_anteriores NVARCHAR(MAX) = NULL,
    @valores_nuevos NVARCHAR(MAX) = NULL,
    @ip_address VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO BitacoraMovimientos 
        (
            usuario_id, 
            accion, 
            tabla, 
            registro_id, 
            valores_anteriores, 
            valores_nuevos, 
            ip_address
        )
        VALUES 
        (
            @usuario_id, 
            @accion, 
            @tabla, 
            @registro_id, 
            @valores_anteriores, 
            @valores_nuevos, 
            @ip_address
        );
        
        RETURN 0; -- Éxito
    END TRY
    BEGIN CATCH
        RETURN -1; -- Error
    END CATCH
END
GO

-- ==================================================
-- SP: sp_CrearProyeccionVenta
-- Descripción: Crea una nueva proyección de venta
-- ==================================================
IF OBJECT_ID('dbo.sp_CrearProyeccionVenta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CrearProyeccionVenta;
GO

CREATE PROCEDURE dbo.sp_CrearProyeccionVenta
    @tienda_id INT,
    @anio INT,
    @mes INT,
    @monto_proyectado DECIMAL(18,2),
    @usuario_id INT,
    @ip_address VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @proyeccion_id INT;
    DECLARE @valores_nuevos NVARCHAR(MAX);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Insertar la proyección
        INSERT INTO ProyeccionVentas 
        (
            tienda_id, 
            anio, 
            mes, 
            monto_proyectado, 
            estado, 
            usuario_creacion_id
        )
        VALUES 
        (
            @tienda_id, 
            @anio, 
            @mes, 
            @monto_proyectado, 
            'ABIERTO', 
            @usuario_id
        );
        
        SET @proyeccion_id = SCOPE_IDENTITY();
        
        -- Crear JSON de valores nuevos
        SET @valores_nuevos = (
            SELECT 
                @tienda_id AS tienda_id,
                @anio AS anio,
                @mes AS mes,
                @monto_proyectado AS monto_proyectado,
                'ABIERTO' AS estado
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
        
        -- Registrar en bitácora
        EXEC sp_RegistrarBitacora 
            @usuario_id, 
            'INSERT', 
            'ProyeccionVentas', 
            @proyeccion_id, 
            NULL, 
            @valores_nuevos, 
            @ip_address;
        
        COMMIT TRANSACTION;
        
        SELECT @proyeccion_id AS id;
        RETURN 0;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
        RETURN -1;
    END CATCH
END
GO

-- ==================================================
-- SP: sp_ActualizarProyeccionVenta
-- Descripción: Actualiza una proyección de venta existente
-- ==================================================
IF OBJECT_ID('dbo.sp_ActualizarProyeccionVenta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ActualizarProyeccionVenta;
GO

CREATE PROCEDURE dbo.sp_ActualizarProyeccionVenta
    @proyeccion_id INT,
    @monto_proyectado DECIMAL(18,2),
    @usuario_id INT,
    @ip_address VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @valores_anteriores NVARCHAR(MAX);
    DECLARE @valores_nuevos NVARCHAR(MAX);
    DECLARE @estado VARCHAR(20);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar que la proyección existe y está ABIERTA
        SELECT @estado = estado
        FROM ProyeccionVentas
        WHERE id = @proyeccion_id;
        
        IF @estado IS NULL
        BEGIN
            THROW 50001, 'Proyección no encontrada', 1;
        END
        
        IF @estado = 'CERRADO'
        BEGIN
            THROW 50002, 'No se puede modificar una proyección cerrada', 1;
        END
        
        -- Capturar valores anteriores
        SET @valores_anteriores = (
            SELECT 
                tienda_id,
                anio,
                mes,
                monto_proyectado,
                estado
            FROM ProyeccionVentas
            WHERE id = @proyeccion_id
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
        
        -- Actualizar la proyección
        UPDATE ProyeccionVentas
        SET 
            monto_proyectado = @monto_proyectado,
            usuario_modificacion_id = @usuario_id,
            fecha_modificacion = GETDATE()
        WHERE id = @proyeccion_id;
        
        -- Capturar valores nuevos
        SET @valores_nuevos = (
            SELECT 
                tienda_id,
                anio,
                mes,
                monto_proyectado,
                estado
            FROM ProyeccionVentas
            WHERE id = @proyeccion_id
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
        
        -- Registrar en bitácora
        EXEC sp_RegistrarBitacora 
            @usuario_id, 
            'UPDATE', 
            'ProyeccionVentas', 
            @proyeccion_id, 
            @valores_anteriores, 
            @valores_nuevos, 
            @ip_address;
        
        COMMIT TRANSACTION;
        RETURN 0;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
        RETURN -1;
    END CATCH
END
GO

-- ==================================================
-- SP: sp_CerrarProyeccionVenta
-- Descripción: Cierra una proyección de venta (cambia estado a CERRADO)
-- ==================================================
IF OBJECT_ID('dbo.sp_CerrarProyeccionVenta', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CerrarProyeccionVenta;
GO

CREATE PROCEDURE dbo.sp_CerrarProyeccionVenta
    @tienda_id INT,
    @anio INT,
    @usuario_id INT,
    @ip_address VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @proyecciones_actualizadas INT = 0;
    DECLARE @valores_anteriores NVARCHAR(MAX);
    DECLARE @valores_nuevos NVARCHAR(MAX);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Actualizar todas las proyecciones de la tienda para el año especificado
        UPDATE ProyeccionVentas
        SET 
            estado = 'CERRADO',
            fecha_cierre = GETDATE(),
            usuario_modificacion_id = @usuario_id,
            fecha_modificacion = GETDATE()
        WHERE tienda_id = @tienda_id 
          AND anio = @anio 
          AND estado = 'ABIERTO';
        
        SET @proyecciones_actualizadas = @@ROWCOUNT;
        
        IF @proyecciones_actualizadas = 0
        BEGIN
            THROW 50003, 'No hay proyecciones abiertas para cerrar', 1;
        END
        
        -- Registrar en bitácora
        SET @valores_nuevos = (
            SELECT 
                @tienda_id AS tienda_id,
                @anio AS anio,
                @proyecciones_actualizadas AS registros_cerrados,
                'CERRADO' AS nuevo_estado
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
        
        EXEC sp_RegistrarBitacora 
            @usuario_id, 
            'CERRAR', 
            'ProyeccionVentas', 
            @tienda_id, 
            NULL, 
            @valores_nuevos, 
            @ip_address;
        
        COMMIT TRANSACTION;
        
        SELECT @proyecciones_actualizadas AS registros_cerrados;
        RETURN 0;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
        RETURN -1;
    END CATCH
END
GO

-- ==================================================
-- SP: sp_ObtenerComparativoVentas
-- Descripción: Obtiene el comparativo de ventas con crecimientos calculados
-- ==================================================
IF OBJECT_ID('dbo.sp_ObtenerComparativoVentas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerComparativoVentas;
GO

CREATE PROCEDURE dbo.sp_ObtenerComparativoVentas
    @tienda_id INT = NULL,
    @anio INT = 2026
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        tienda_id,
        tienda_nombre,
        tienda_codigo,
        pais,
        mes,
        ventas_2024,
        ventas_2025,
        proyeccion_2026,
        crecimiento_2024_vs_2025,
        crecimiento_2025_vs_2026,
        estado_proyeccion
    FROM vw_ComparativoVentas
    WHERE (@tienda_id IS NULL OR tienda_id = @tienda_id)
    ORDER BY tienda_id, mes;
END
GO

-- ==================================================
-- SP: sp_ObtenerBitacora
-- Descripción: Obtiene registros de la bitácora con filtros
-- ==================================================
IF OBJECT_ID('dbo.sp_ObtenerBitacora', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerBitacora;
GO

CREATE PROCEDURE dbo.sp_ObtenerBitacora
    @usuario_id INT = NULL,
    @tabla VARCHAR(50) = NULL,
    @fecha_desde DATETIME = NULL,
    @fecha_hasta DATETIME = NULL,
    @top INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@top)
        b.id,
        b.usuario_id,
        u.username,
        u.nombre_completo,
        b.accion,
        b.tabla,
        b.registro_id,
        b.valores_anteriores,
        b.valores_nuevos,
        b.ip_address,
        b.fecha_accion
    FROM BitacoraMovimientos b
    INNER JOIN Usuarios u ON b.usuario_id = u.id
    WHERE (@usuario_id IS NULL OR b.usuario_id = @usuario_id)
      AND (@tabla IS NULL OR b.tabla = @tabla)
      AND (@fecha_desde IS NULL OR b.fecha_accion >= @fecha_desde)
      AND (@fecha_hasta IS NULL OR b.fecha_accion <= @fecha_hasta)
    ORDER BY b.fecha_accion DESC;
END
GO

PRINT 'Stored Procedures creados exitosamente';
PRINT '--------------------------------------';
PRINT 'Procedures disponibles:';
PRINT '  - sp_RegistrarBitacora';
PRINT '  - sp_CrearProyeccionVenta';
PRINT '  - sp_ActualizarProyeccionVenta';
PRINT '  - sp_CerrarProyeccionVenta';
PRINT '  - sp_ObtenerComparativoVentas';
PRINT '  - sp_ObtenerBitacora';
GO

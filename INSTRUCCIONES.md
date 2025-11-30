# Sistema de Proyección de Ventas - Los Ladrillos S.A.

## Iniciar la Aplicación

```bash
docker compose up -d
```

**⚠️ Importante**: Esperar aproximadamente 40-50 segundos para que todos los servicios se inicialicen correctamente (SQL Server + Backend + Creación automática de usuarios).

## Acceder a la Aplicación

- **Frontend**: http://localhost
- **Backend API**: http://localhost/api
- **Swagger**: http://localhost/swagger

## Credenciales de Acceso

### Usuario Gerente
- **Usuario**: `gerente`
- **Contraseña**: `gerente123`
- **Rol**: GERENTE

### Usuario Admin
- **Usuario**: `admin`
- **Contraseña**: `admin123`
- **Rol**: ADMIN

## Servicios Docker

- **SQL Server 2022**: Puerto 1433
- **Backend .NET 8**: Puerto 5001
- **Frontend React**: Puerto 3000
- **Nginx**: Puerto 80 (proxy inverso)

## Detener la Aplicación

```bash
docker compose down
```

## Reiniciar Completamente (con datos limpios)

```bash
docker compose down -v
docker compose up -d --build
```

## Ver Logs

```bash
# Ver todos los logs
docker compose logs

# Ver logs de un servicio específico
docker compose logs backend
docker compose logs sqlserver
docker compose logs frontend
```

## Estructura del Proyecto

- `/backend` - API .NET 8 con Entity Framework Core
- `/frontend` - Aplicación React con Vite
- `/database` - Scripts SQL para inicialización
- `/nginx` - Configuración del proxy inverso

## Características

- Autenticación JWT
- Gestión de proyecciones de ventas por tienda y año
- Comparación de ventas históricas (2024, 2025) vs proyecciones (2026)
- Auditoría completa de cambios (bitácora)
- Multi-país con soporte de diferentes monedas

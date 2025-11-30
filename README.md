# Sistema de Proyección de Ventas 2026

Sistema para gestionar proyecciones de ventas de Los Ladrillos S.A. (materiales de construcción en Guatemala). Registra proyecciones mensuales, compara con datos históricos y mantiene auditoría completa.

## Características principales

- Ingreso y edición de proyecciones mensuales por tienda
- Comparación automática 2024 vs 2025 vs proyección 2026
- Cálculo de porcentajes de crecimiento
- Sistema de cierre de proyecciones (estados ABIERTO/CERRADO)
- Bitácora de auditoría completa
- Control de acceso por roles (GERENTE)

## Stack técnico

- **Backend**: .NET 8 Web API + Entity Framework Core
- **Base de datos**: SQL Server 2022
- **Frontend**: React 18 + Vite
- **Infraestructura**: Docker + Docker Compose + Nginx

## Instalación

Necesitas Docker Desktop.

```bash
git clone https://github.com/Tuki1077/prueba-tecnica-desarrollador-SM.git
cd prueba-tecnica-desarrollador-SM
docker-compose up --build
```

Espera 1-2 minutos mientras se inicializa todo.

## Acceso

Abre http://localhost

**Usuarios:**
- `gerente` / `gerente123`
- `admin` / `admin123`

## Cómo usar

1. Inicia sesión con las credenciales
2. Selecciona año 2026 y una tienda
3. Click en las celdas para agregar o editar montos
4. Guarda con ✓ o cancela con ✗
5. Usa "Cerrar Proyección" cuando termines
6. Revisa la bitácora para ver el historial

## Estructura

```
├── backend/              # API .NET 8
│   ├── Controllers/     # Endpoints
│   ├── Models/         # Entidades
│   ├── Services/       # Lógica de negocio
│   └── Data/          # DbContext
├── frontend/           # React + Vite
│   └── src/
│       ├── components/ # Login, Proyecciones, Bitácora
│       ├── services/  # Cliente API
│       └── styles/    # CSS global
├── database/          # Scripts SQL
│   ├── 01-init.sql
│   ├── 02-functions.sql
│   ├── 03-data.sql
│   └── 04-procedures.sql
└── docker-compose.yml
```

## Base de datos

**Tablas:**
- Paises, Tiendas, Usuarios
- VentasHistoricas (2024-2025)
- ProyeccionVentas (2026)
- BitacoraMovimientos

**Funciones SQL:**
- `fn_CrecimientoVentas2024vs2025`: calcula % de crecimiento
- `fn_CrecimientoProyeccion2026vs2025`: calcula % proyección

Ver diagrama en `database/diagram.mmd` (Mermaid).

## API

```
POST   /api/auth/login
POST   /api/auth/register

GET    /api/proyecciones/{año}/{tiendaId}
POST   /api/proyecciones
PUT    /api/proyecciones
POST   /api/proyecciones/cerrar/{año}/{tiendaId}

GET    /api/bitacora
GET    /api/tiendas
```

## Desarrollo local

```bash
# Backend
cd backend/LosLadrillosAPI
dotnet run

# Frontend
cd frontend
npm install
npm run dev
```

## Comandos útiles

```bash
docker-compose logs -f        # Ver logs
docker-compose restart        # Reiniciar
docker-compose down          # Detener
docker-compose down -v       # Detener y limpiar datos
```

## Troubleshooting

**SQL Server tarda en iniciar**: Dale 40-50 segundos, es normal.

**Backend no conecta**: Verifica que SQL Server esté corriendo: `docker-compose ps`

**Puerto 80 ocupado**: Cambia el puerto en `docker-compose.yml`:
```yaml
nginx:
  ports:
    - "8080:80"
```

## Documentación

- `PLANIFICACION.md`: Planificación completa del proyecto

## Licencia

Proyecto de prueba técnica.

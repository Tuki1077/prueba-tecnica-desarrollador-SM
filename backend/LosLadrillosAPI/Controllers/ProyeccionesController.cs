using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LosLadrillosAPI.Data;
using LosLadrillosAPI.Models;
using LosLadrillosAPI.DTOs;
using LosLadrillosAPI.Services;
using System.Security.Claims;
using System.Text.Json;

namespace LosLadrillosAPI.Controllers
{
    [Authorize(Roles = "GERENTE,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProyeccionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;

        public ProyeccionesController(ApplicationDbContext context, IBitacoraService bitacoraService)
        {
            _context = context;
            _bitacoraService = bitacoraService;
        }

        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/proyecciones/{anio}/{tiendaId}
        [HttpGet("{anio}/{tiendaId}")]
        public async Task<ActionResult<ResumenProyeccionDto>> ObtenerResumenProyeccion(int anio, int tiendaId)
        {
            var tienda = await _context.Tiendas
                .Include(t => t.Pais)
                .FirstOrDefaultAsync(t => t.TiendaId == tiendaId);

            if (tienda == null)
                return NotFound(new { message = "Tienda no encontrada" });

            var meses = new List<MesProyeccionDto>();
            var mesesNombres = new[] { "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", 
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

            var ventas2024 = await _context.VentasHistoricas
                .Where(v => v.TiendaId == tiendaId && v.Anio == 2024)
                .ToListAsync();

            var ventas2025 = await _context.VentasHistoricas
                .Where(v => v.TiendaId == tiendaId && v.Anio == 2025)
                .ToListAsync();

            var proyecciones2026 = await _context.ProyeccionesVentas
                .Where(p => p.TiendaId == tiendaId && p.Anio == anio)
                .ToListAsync();

            for (int mes = 1; mes <= 12; mes++)
            {
                var venta2024 = ventas2024.FirstOrDefault(v => v.Mes == mes);
                var venta2025 = ventas2025.FirstOrDefault(v => v.Mes == mes);
                var proyeccion = proyecciones2026.FirstOrDefault(p => p.Mes == mes);

                decimal? porcentajeCrecimiento = null;
                if (venta2025 != null && proyeccion != null && venta2025.MontoVenta > 0)
                {
                    porcentajeCrecimiento = ((proyeccion.MontoProyectado - venta2025.MontoVenta) / venta2025.MontoVenta) * 100;
                }

                meses.Add(new MesProyeccionDto
                {
                    Mes = mes,
                    NombreMes = mesesNombres[mes],
                    Venta2024 = venta2024?.MontoVenta,
                    Venta2025 = venta2025?.MontoVenta,
                    Proyeccion2026 = proyeccion?.MontoProyectado,
                    PorcentajeCrecimiento = porcentajeCrecimiento
                });
            }

            var totalProyectado = proyecciones2026.Sum(p => p.MontoProyectado);
            var total2024 = ventas2024.Sum(v => v.MontoVenta);
            var total2025 = ventas2025.Sum(v => v.MontoVenta);

            decimal? porcentajeCrecimiento2024vs2025 = null;
            if (total2024 > 0 && total2025 > 0)
            {
                porcentajeCrecimiento2024vs2025 = ((total2025 - total2024) / total2024) * 100;
            }

            decimal? porcentajeCrecimiento2025vs2026 = null;
            if (total2025 > 0 && totalProyectado > 0)
            {
                porcentajeCrecimiento2025vs2026 = ((totalProyectado - total2025) / total2025) * 100;
            }

            var estado = proyecciones2026.FirstOrDefault()?.Estado ?? "ABIERTO";

            return Ok(new ResumenProyeccionDto
            {
                Anio = anio,
                TiendaId = tiendaId,
                CodigoTienda = tienda.CodigoTienda,
                NombreTienda = tienda.NombreTienda,
                Moneda = tienda.Pais.CodigoMoneda,
                Estado = estado,
                Meses = meses,
                TotalProyectado = totalProyectado,
                PorcentajeCrecimiento2024vs2025 = porcentajeCrecimiento2024vs2025,
                PorcentajeCrecimiento2025vs2026 = porcentajeCrecimiento2025vs2026
            });
        }

        // POST: api/proyecciones
        [HttpPost]
        public async Task<ActionResult<ProyeccionVenta>> CrearProyeccion([FromBody] ProyeccionVentaCreateDto dto)
        {
            // Verificar si ya existe
            var existe = await _context.ProyeccionesVentas
                .AnyAsync(p => p.TiendaId == dto.TiendaId && p.Anio == dto.Anio && p.Mes == dto.Mes);

            if (existe)
                return BadRequest(new { message = "Ya existe una proyección para esta tienda, año y mes" });

            var proyeccion = new ProyeccionVenta
            {
                TiendaId = dto.TiendaId,
                Anio = dto.Anio,
                Mes = dto.Mes,
                MontoProyectado = dto.MontoProyectado,
                Estado = "ABIERTO",
                UsuarioCreacionId = ObtenerUsuarioId(),
                FechaCreacion = DateTime.Now
            };

            _context.ProyeccionesVentas.Add(proyeccion);
            await _context.SaveChangesAsync();

            // Registrar en bitácora
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var valores = JsonSerializer.Serialize(dto);
            await _bitacoraService.RegistrarAccionAsync(ObtenerUsuarioId(), "INSERT", "ProyeccionesVentas", 
                proyeccion.ProyeccionVentaId, null, valores, ip);

            return CreatedAtAction(nameof(ObtenerResumenProyeccion), 
                new { anio = proyeccion.Anio, tiendaId = proyeccion.TiendaId }, proyeccion);
        }

        // PUT: api/proyecciones
        [HttpPut]
        public async Task<ActionResult> ActualizarProyeccion([FromBody] ProyeccionVentaUpdateDto dto)
        {
            var proyeccion = await _context.ProyeccionesVentas
                .FirstOrDefaultAsync(p => p.ProyeccionVentaId == dto.ProyeccionVentaId);

            if (proyeccion == null)
                return NotFound(new { message = "Proyección no encontrada" });

            if (proyeccion.Estado == "CERRADO")
                return BadRequest(new { message = "No se puede modificar una proyección cerrada" });

            var valoresAnteriores = JsonSerializer.Serialize(new { proyeccion.MontoProyectado });
            
            proyeccion.MontoProyectado = dto.MontoProyectado;
            proyeccion.UsuarioModificacionId = ObtenerUsuarioId();
            proyeccion.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var valoresNuevos = JsonSerializer.Serialize(new { proyeccion.MontoProyectado });
            await _bitacoraService.RegistrarAccionAsync(ObtenerUsuarioId(), "UPDATE", "ProyeccionesVentas", 
                proyeccion.ProyeccionVentaId, valoresAnteriores, valoresNuevos, ip);

            return NoContent();
        }

        // POST: api/proyecciones/cerrar/{anio}/{tiendaId}
        [HttpPost("cerrar/{anio}/{tiendaId}")]
        public async Task<ActionResult> CerrarProyeccion(int anio, int tiendaId)
        {
            var proyecciones = await _context.ProyeccionesVentas
                .Where(p => p.TiendaId == tiendaId && p.Anio == anio)
                .ToListAsync();

            if (!proyecciones.Any())
                return NotFound(new { message = "No se encontraron proyecciones para cerrar" });

            var usuarioId = ObtenerUsuarioId();
            var fechaCierre = DateTime.Now;

            foreach (var proyeccion in proyecciones)
            {
                if (proyeccion.Estado == "ABIERTO")
                {
                    proyeccion.Estado = "CERRADO";
                    proyeccion.FechaCierre = fechaCierre;
                }
            }

            await _context.SaveChangesAsync();

            // Registrar en bitácora
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _bitacoraService.RegistrarAccionAsync(usuarioId, "CIERRE", "ProyeccionesVentas", 
                tiendaId, null, $"Cerradas {proyecciones.Count} proyecciones del año {anio}", ip);

            return Ok(new { message = $"Se cerraron {proyecciones.Count} proyecciones exitosamente" });
        }

        // GET: api/proyecciones/crecimiento/{tiendaId}
        [HttpGet("crecimiento/{tiendaId}")]
        public async Task<ActionResult<CrecimientoDto>> ObtenerCrecimiento(int tiendaId)
        {
            var total2024 = await _context.VentasHistoricas
                .Where(v => v.TiendaId == tiendaId && v.Anio == 2024)
                .SumAsync(v => v.MontoVenta);

            var total2025 = await _context.VentasHistoricas
                .Where(v => v.TiendaId == tiendaId && v.Anio == 2025)
                .SumAsync(v => v.MontoVenta);

            var total2026 = await _context.ProyeccionesVentas
                .Where(p => p.TiendaId == tiendaId && p.Anio == 2026)
                .SumAsync(p => p.MontoProyectado);

            decimal? crecimiento2024vs2025 = null;
            if (total2024 > 0)
            {
                crecimiento2024vs2025 = ((total2025 - total2024) / total2024) * 100;
            }

            decimal? crecimiento2025vs2026 = null;
            if (total2025 > 0)
            {
                crecimiento2025vs2026 = ((total2026 - total2025) / total2025) * 100;
            }

            return Ok(new CrecimientoDto
            {
                PorcentajeCrecimiento2024vs2025 = crecimiento2024vs2025,
                PorcentajeCrecimiento2025vs2026 = crecimiento2025vs2026,
                TotalVentas2024 = total2024,
                TotalVentas2025 = total2025,
                TotalProyeccion2026 = total2026
            });
        }
    }
}

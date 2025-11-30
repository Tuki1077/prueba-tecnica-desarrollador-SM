using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LosLadrillosAPI.Data;
using LosLadrillosAPI.Models;

namespace LosLadrillosAPI.Controllers
{
    [Authorize(Roles = "GERENTE,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class TiendasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TiendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/tiendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tienda>>> ObtenerTiendas()
        {
            return await _context.Tiendas
                .Include(t => t.Pais)
                .Where(t => t.Activa)
                .OrderBy(t => t.CodigoTienda)
                .ToListAsync();
        }

        // GET: api/tiendas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tienda>> ObtenerTienda(int id)
        {
            var tienda = await _context.Tiendas
                .Include(t => t.Pais)
                .FirstOrDefaultAsync(t => t.TiendaId == id);

            if (tienda == null)
                return NotFound();

            return tienda;
        }

        // GET: api/tiendas/pais/{codigoPais}
        [HttpGet("pais/{codigoPais}")]
        public async Task<ActionResult<IEnumerable<Tienda>>> ObtenerTiendasPorPais(string codigoPais)
        {
            var pais = await _context.Paises
                .FirstOrDefaultAsync(p => p.CodigoPais == codigoPais);

            if (pais == null)
                return NotFound(new { message = "País no encontrado" });

            var tiendas = await _context.Tiendas
                .Include(t => t.Pais)
                .Where(t => t.PaisId == pais.PaisId && t.Activa)
                .OrderBy(t => t.CodigoTienda)
                .ToListAsync();

            return tiendas;
        }
    }
}

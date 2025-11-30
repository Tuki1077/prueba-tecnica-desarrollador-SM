using LosLadrillosAPI.Data;
using LosLadrillosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LosLadrillosAPI.Services
{
    public interface IBitacoraService
    {
        Task RegistrarAccionAsync(int usuarioId, string accion, string tabla, int? registroId = null, 
            string? valoresAnteriores = null, string? valoresNuevos = null, string? direccionIP = null);
        Task<List<Bitacora>> ObtenerBitacorasAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, 
            int? usuarioId = null, string? accion = null);
    }

    public class BitacoraService : IBitacoraService
    {
        private readonly ApplicationDbContext _context;

        public BitacoraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAccionAsync(int usuarioId, string accion, string tabla, 
            int? registroId = null, string? valoresAnteriores = null, string? valoresNuevos = null, 
            string? direccionIP = null)
        {
            var bitacora = new Bitacora
            {
                UsuarioId = usuarioId,
                Accion = accion,
                Tabla = tabla,
                RegistroId = registroId,
                ValoresAnteriores = valoresAnteriores,
                ValoresNuevos = valoresNuevos,
                DireccionIP = direccionIP,
                FechaHora = DateTime.Now
            };

            _context.Bitacoras.Add(bitacora);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bitacora>> ObtenerBitacorasAsync(DateTime? fechaInicio = null, 
            DateTime? fechaFin = null, int? usuarioId = null, string? accion = null)
        {
            var query = _context.Bitacoras
                .Include(b => b.Usuario)
                .AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(b => b.FechaHora >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(b => b.FechaHora <= fechaFin.Value);

            if (usuarioId.HasValue)
                query = query.Where(b => b.UsuarioId == usuarioId.Value);

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(b => b.Accion == accion);

            return await query.OrderByDescending(b => b.FechaHora).ToListAsync();
        }
    }
}

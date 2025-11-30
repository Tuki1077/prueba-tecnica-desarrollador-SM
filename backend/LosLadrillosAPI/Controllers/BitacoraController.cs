using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LosLadrillosAPI.Services;
using LosLadrillosAPI.DTOs;

namespace LosLadrillosAPI.Controllers
{
    [Authorize(Roles = "GERENTE,ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class BitacoraController : ControllerBase
    {
        private readonly IBitacoraService _bitacoraService;

        public BitacoraController(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        // GET: api/bitacora
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BitacoraDto>>> ObtenerBitacoras(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] int? usuarioId = null,
            [FromQuery] string? accion = null)
        {
            var bitacoras = await _bitacoraService.ObtenerBitacorasAsync(fechaInicio, fechaFin, usuarioId, accion);

            var bitacorasDto = bitacoras.Select(b => new BitacoraDto
            {
                BitacoraId = b.BitacoraId,
                NombreUsuario = b.Usuario.NombreUsuario,
                Accion = b.Accion,
                Tabla = b.Tabla,
                RegistroId = b.RegistroId,
                ValoresAnteriores = b.ValoresAnteriores,
                ValoresNuevos = b.ValoresNuevos,
                FechaHora = b.FechaHora,
                DireccionIP = b.DireccionIP
            }).ToList();

            return Ok(bitacorasDto);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LosLadrillosAPI.Data;
using LosLadrillosAPI.Models;
using LosLadrillosAPI.DTOs;
using LosLadrillosAPI.Services;

namespace LosLadrillosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IBitacoraService _bitacoraService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, IBitacoraService bitacoraService)
        {
            _context = context;
            _jwtService = jwtService;
            _bitacoraService = bitacoraService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario && u.Activo);

            if (usuario == null)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            // Verificar password con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            // Generar token JWT
            var token = _jwtService.GenerateToken(usuario);

            // Registrar en bitácora
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _bitacoraService.RegistrarAccionAsync(usuario.UsuarioId, "LOGIN", "Usuarios", 
                usuario.UsuarioId, null, null, ip);

            return Ok(new LoginResponseDto
            {
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                UsuarioId = usuario.UsuarioId
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<Usuario>> Register([FromBody] Usuario usuario)
        {
            // Verificar si ya existe el usuario
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario))
                return BadRequest(new { message = "El nombre de usuario ya existe" });

            // Hash del password
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            usuario.FechaCreacion = DateTime.Now;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = usuario.UsuarioId }, usuario);
        }
    }
}

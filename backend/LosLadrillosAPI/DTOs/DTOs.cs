namespace LosLadrillosAPI.DTOs
{
    public class LoginRequestDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }

    public class RegisterRequestDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; }
        public string Rol { get; set; } = "GERENTE";
    }

    public class ProyeccionVentaDto
    {
        public int ProyeccionVentaId { get; set; }
        public int TiendaId { get; set; }
        public string CodigoTienda { get; set; } = string.Empty;
        public string NombreTienda { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Mes { get; set; }
        public decimal MontoProyectado { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaCierre { get; set; }
    }

    public class ProyeccionVentaCreateDto
    {
        public int TiendaId { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public decimal MontoProyectado { get; set; }
    }

    public class ProyeccionVentaUpdateDto
    {
        public int ProyeccionVentaId { get; set; }
        public decimal MontoProyectado { get; set; }
    }

    public class ResumenProyeccionDto
    {
        public int Anio { get; set; }
        public int TiendaId { get; set; }
        public string CodigoTienda { get; set; } = string.Empty;
        public string NombreTienda { get; set; } = string.Empty;
        public string Moneda { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public List<MesProyeccionDto> Meses { get; set; } = new();
        public decimal TotalProyectado { get; set; }
        public decimal? PorcentajeCrecimiento2024vs2025 { get; set; }
        public decimal? PorcentajeCrecimiento2025vs2026 { get; set; }
    }

    public class MesProyeccionDto
    {
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public decimal? Venta2024 { get; set; }
        public decimal? Venta2025 { get; set; }
        public decimal? Proyeccion2026 { get; set; }
        public decimal? PorcentajeCrecimiento { get; set; }
    }

    public class CrecimientoDto
    {
        public decimal? PorcentajeCrecimiento2024vs2025 { get; set; }
        public decimal? PorcentajeCrecimiento2025vs2026 { get; set; }
        public decimal TotalVentas2024 { get; set; }
        public decimal TotalVentas2025 { get; set; }
        public decimal TotalProyeccion2026 { get; set; }
    }

    public class BitacoraDto
    {
        public int BitacoraId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Tabla { get; set; } = string.Empty;
        public int? RegistroId { get; set; }
        public string? ValoresAnteriores { get; set; }
        public string? ValoresNuevos { get; set; }
        public DateTime FechaHora { get; set; }
        public string? DireccionIP { get; set; }
    }
}

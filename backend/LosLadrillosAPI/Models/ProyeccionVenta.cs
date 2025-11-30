using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("ProyeccionesVentas")]
    public class ProyeccionVenta
    {
        [Key]
        public int ProyeccionVentaId { get; set; }

        [Required]
        public int TiendaId { get; set; }

        [Required]
        public int Anio { get; set; }

        [Required]
        public int Mes { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoProyectado { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "ABIERTO"; // ABIERTO, CERRADO

        public int? UsuarioRegistroId { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public int? UsuarioModificacionId { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioCierreId { get; set; }
        public DateTime? FechaCierre { get; set; }

        // Relaciones
        [ForeignKey("TiendaId")]
        public virtual Tienda Tienda { get; set; } = null!;

        [ForeignKey("UsuarioRegistroId")]
        public virtual Usuario? UsuarioRegistro { get; set; }

        [ForeignKey("UsuarioModificacionId")]
        public virtual Usuario? UsuarioModificacion { get; set; }

        [ForeignKey("UsuarioCierreId")]
        public virtual Usuario? UsuarioCierre { get; set; }
    }
}

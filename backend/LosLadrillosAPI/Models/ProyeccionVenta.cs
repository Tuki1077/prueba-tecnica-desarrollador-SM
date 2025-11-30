using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("ProyeccionVentas")]
    public class ProyeccionVenta
    {
        [Key]
        [Column("id")]
        public int ProyeccionVentaId { get; set; }

        [Required]
        [Column("tienda_id")]
        public int TiendaId { get; set; }

        [Required]
        [Column("anio")]
        public int Anio { get; set; }

        [Required]
        [Column("mes")]
        public int Mes { get; set; }

        [Required]
        [Column("monto_proyectado", TypeName = "decimal(18,2)")]
        public decimal MontoProyectado { get; set; }

        [Required]
        [StringLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "ABIERTO"; // ABIERTO, CERRADO

        [Column("usuario_creacion_id")]
        public int? UsuarioCreacionId { get; set; }
        
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        [Column("usuario_modificacion_id")]
        public int? UsuarioModificacionId { get; set; }
        
        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }
        
        [Column("fecha_cierre")]
        public DateTime? FechaCierre { get; set; }

        // Relaciones
        [ForeignKey("TiendaId")]
        public virtual Tienda Tienda { get; set; } = null!;

        [ForeignKey("UsuarioCreacionId")]
        public virtual Usuario? UsuarioCreacion { get; set; }

        [ForeignKey("UsuarioModificacionId")]
        public virtual Usuario? UsuarioModificacion { get; set; }
    }
}

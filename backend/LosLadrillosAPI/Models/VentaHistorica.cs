using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("VentasHistoricas")]
    public class VentaHistorica
    {
        [Key]
        [Column("id")]
        public int VentaHistoricaId { get; set; }

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
        [Column("monto", TypeName = "decimal(18,2)")]
        public decimal MontoVenta { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_modificacion")]
        public DateTime? FechaModificacion { get; set; }

        // Relaciones
        [ForeignKey("TiendaId")]
        public virtual Tienda Tienda { get; set; } = null!;
    }
}

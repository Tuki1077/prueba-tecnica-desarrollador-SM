using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("VentasHistoricas")]
    public class VentaHistorica
    {
        [Key]
        public int VentaHistoricaId { get; set; }

        [Required]
        public int TiendaId { get; set; }

        [Required]
        public int Anio { get; set; }

        [Required]
        public int Mes { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoVenta { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("TiendaId")]
        public virtual Tienda Tienda { get; set; } = null!;
    }
}

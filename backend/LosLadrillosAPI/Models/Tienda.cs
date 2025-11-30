using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("Tiendas")]
    public class Tienda
    {
        [Key]
        public int TiendaId { get; set; }

        [Required]
        [StringLength(10)]
        public string CodigoTienda { get; set; } = string.Empty; // 10, 20, 30

        [Required]
        [StringLength(100)]
        public string NombreTienda { get; set; } = string.Empty; // Ciudad, Petén, Xela

        [Required]
        public int PaisId { get; set; }

        [Required]
        public bool Activa { get; set; } = true;

        // Relaciones
        [ForeignKey("PaisId")]
        public virtual Pais Pais { get; set; } = null!;

        public virtual ICollection<VentaHistorica> VentasHistoricas { get; set; } = new List<VentaHistorica>();
        public virtual ICollection<ProyeccionVenta> ProyeccionesVentas { get; set; } = new List<ProyeccionVenta>();
    }
}

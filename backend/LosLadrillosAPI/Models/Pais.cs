using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("Paises")]
    public class Pais
    {
        [Key]
        public int PaisId { get; set; }

        [Required]
        [StringLength(3)]
        public string CodigoPais { get; set; } = string.Empty; // GT, SV, HN, NI, MX

        [Required]
        [StringLength(100)]
        public string NombrePais { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string CodigoMoneda { get; set; } = string.Empty; // GTQ, USD, HNL, NIO, MXN

        [Required]
        [StringLength(50)]
        public string NombreMoneda { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual ICollection<Tienda> Tiendas { get; set; } = new List<Tienda>();
    }
}

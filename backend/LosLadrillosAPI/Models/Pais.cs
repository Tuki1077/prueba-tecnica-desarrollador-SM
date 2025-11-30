using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("Paises")]
    public class Pais
    {
        [Key]
        [Column("id")]
        public int PaisId { get; set; }

        [Required]
        [StringLength(3)]
        [Column("codigo")]
        public string CodigoPais { get; set; } = string.Empty; // GT, SV, HN, NI, MX

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string NombrePais { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("moneda")]
        public string CodigoMoneda { get; set; } = string.Empty; // Ej: "Quetzales (GTQ)", "Dólares (USD)"

        [Required]
        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual ICollection<Tienda> Tiendas { get; set; } = new List<Tienda>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("Bitacoras")]
    public class Bitacora
    {
        [Key]
        public int BitacoraId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty; // INSERT, UPDATE, DELETE, LOGIN, CIERRE

        [Required]
        [StringLength(100)]
        public string Tabla { get; set; } = string.Empty;

        public int? RegistroId { get; set; }

        [StringLength(1000)]
        public string? ValoresAnteriores { get; set; }

        [StringLength(1000)]
        public string? ValoresNuevos { get; set; }

        [Required]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? DireccionIP { get; set; }

        // Relaciones
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}

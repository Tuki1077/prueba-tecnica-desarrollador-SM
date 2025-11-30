using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "GERENTE"; // GERENTE, ADMIN

        [Required]
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    }
}

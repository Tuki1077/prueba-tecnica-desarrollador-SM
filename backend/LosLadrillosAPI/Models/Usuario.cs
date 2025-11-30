using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("username")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(150)]
        [Column("nombre_completo")]
        public string? NombreCompleto { get; set; }

        [Required]
        [StringLength(20)]
        [Column("rol")]
        public string Rol { get; set; } = "GERENTE"; // GERENTE, ADMIN

        [Required]
        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual ICollection<Bitacora> Bitacoras { get; set; } = new List<Bitacora>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LosLadrillosAPI.Models
{
    [Table("BitacoraMovimientos")]
    public class Bitacora
    {
        [Key]
        [Column("id")]
        public int BitacoraId { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("accion")]
        public string Accion { get; set; } = string.Empty; // INSERT, UPDATE, DELETE, CERRAR, ABRIR

        [Required]
        [StringLength(50)]
        [Column("tabla")]
        public string Tabla { get; set; } = string.Empty;

        [Required]
        [Column("registro_id")]
        public int RegistroId { get; set; }

        [Column("valores_anteriores")]
        public string? ValoresAnteriores { get; set; }

        [Column("valores_nuevos")]
        public string? ValoresNuevos { get; set; }

        [Column("ip_address")]
        public string? DireccionIP { get; set; }

        [Column("fecha_accion")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        // Relaciones
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}

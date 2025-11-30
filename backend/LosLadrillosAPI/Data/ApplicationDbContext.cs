using Microsoft.EntityFrameworkCore;
using LosLadrillosAPI.Models;

namespace LosLadrillosAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Tienda> Tiendas { get; set; }
        public DbSet<VentaHistorica> VentasHistoricas { get; set; }
        public DbSet<ProyeccionVenta> ProyeccionesVentas { get; set; }
        public DbSet<Bitacora> Bitacoras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relaciones
            modelBuilder.Entity<ProyeccionVenta>()
                .HasOne(p => p.UsuarioCreacion)
                .WithMany()
                .HasForeignKey(p => p.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProyeccionVenta>()
                .HasOne(p => p.UsuarioModificacion)
                .WithMany()
                .HasForeignKey(p => p.UsuarioModificacionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Índices únicos
            modelBuilder.Entity<VentaHistorica>()
                .HasIndex(v => new { v.TiendaId, v.Anio, v.Mes })
                .IsUnique();

            modelBuilder.Entity<ProyeccionVenta>()
                .HasIndex(p => new { p.TiendaId, p.Anio, p.Mes })
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Tienda>()
                .HasIndex(t => new { t.CodigoTienda, t.PaisId })
                .IsUnique();
        }
    }
}

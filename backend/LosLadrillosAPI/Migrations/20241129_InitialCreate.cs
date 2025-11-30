using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LosLadrillosAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paises",
                columns: table => new
                {
                    PaisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPais = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NombrePais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoMoneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NombreMoneda = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paises", x => x.PaisId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Tiendas",
                columns: table => new
                {
                    TiendaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoTienda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NombreTienda = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaisId = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas", x => x.TiendaId);
                    table.ForeignKey(
                        name: "FK_Tiendas_Paises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "Paises",
                        principalColumn: "PaisId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bitacoras",
                columns: table => new
                {
                    BitacoraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tabla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistroId = table.Column<int>(type: "int", nullable: true),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitacoras", x => x.BitacoraId);
                    table.ForeignKey(
                        name: "FK_Bitacoras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VentasHistoricas",
                columns: table => new
                {
                    VentaHistoricaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TiendaId = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    MontoVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentasHistoricas", x => x.VentaHistoricaId);
                    table.ForeignKey(
                        name: "FK_VentasHistoricas_Tiendas_TiendaId",
                        column: x => x.TiendaId,
                        principalTable: "Tiendas",
                        principalColumn: "TiendaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyeccionesVentas",
                columns: table => new
                {
                    ProyeccionVentaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TiendaId = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    MontoProyectado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UsuarioRegistroId = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCierreId = table.Column<int>(type: "int", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyeccionesVentas", x => x.ProyeccionVentaId);
                    table.ForeignKey(
                        name: "FK_ProyeccionesVentas_Tiendas_TiendaId",
                        column: x => x.TiendaId,
                        principalTable: "Tiendas",
                        principalColumn: "TiendaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyeccionesVentas_Usuarios_UsuarioRegistroId",
                        column: x => x.UsuarioRegistroId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_ProyeccionesVentas_Usuarios_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_ProyeccionesVentas_Usuarios_UsuarioCierreId",
                        column: x => x.UsuarioCierreId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bitacoras_UsuarioId",
                table: "Bitacoras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyeccionesVentas_TiendaId_Anio_Mes",
                table: "ProyeccionesVentas",
                columns: new[] { "TiendaId", "Anio", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProyeccionesVentas_UsuarioCierreId",
                table: "ProyeccionesVentas",
                column: "UsuarioCierreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyeccionesVentas_UsuarioModificacionId",
                table: "ProyeccionesVentas",
                column: "UsuarioModificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyeccionesVentas_UsuarioRegistroId",
                table: "ProyeccionesVentas",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_CodigoTienda_PaisId",
                table: "Tiendas",
                columns: new[] { "CodigoTienda", "PaisId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_PaisId",
                table: "Tiendas",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VentasHistoricas_TiendaId_Anio_Mes",
                table: "VentasHistoricas",
                columns: new[] { "TiendaId", "Anio", "Mes" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Bitacoras");
            migrationBuilder.DropTable(name: "ProyeccionesVentas");
            migrationBuilder.DropTable(name: "VentasHistoricas");
            migrationBuilder.DropTable(name: "Tiendas");
            migrationBuilder.DropTable(name: "Paises");
            migrationBuilder.DropTable(name: "Usuarios");
        }
    }
}

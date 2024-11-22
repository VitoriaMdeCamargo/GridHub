using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GridHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class vAdicionando_Tb_Microgrid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Microgrids",
                columns: table => new
                {
                    MicrogridId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EspacoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NomeMicrogrid = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    FotoMicrogrid = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    RadiacaoSolarNecessaria = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TopografiaNecessaria = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    AreaTotalNecessaria = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    VelocidadeVentoNecessaria = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    FonteEnergia = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    MetaFinanciamento = table.Column<double>(type: "BINARY_DOUBLE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microgrids", x => x.MicrogridId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Microgrids");
        }
    }
}

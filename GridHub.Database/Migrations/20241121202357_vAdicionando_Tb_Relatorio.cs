using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GridHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class vAdicionando_Tb_Relatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Relatorio",
                columns: table => new
                {
                    RelatorioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MicrogridId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EnergiaGerada = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TempPainelSolar = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LucroGerado = table.Column<double>(type: "BINARY_DOUBLE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorio", x => x.RelatorioId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Relatorio");
        }
    }
}

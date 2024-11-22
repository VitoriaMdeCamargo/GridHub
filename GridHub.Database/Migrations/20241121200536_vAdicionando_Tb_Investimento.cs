using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GridHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class vAdicionando_Tb_Investimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Investimento",
                columns: table => new
                {
                    InvestimentoId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MicrogridId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DescricaoProposta = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investimento", x => x.InvestimentoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investimento");
        }
    }
}

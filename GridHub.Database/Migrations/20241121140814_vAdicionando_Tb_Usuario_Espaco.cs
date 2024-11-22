using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GridHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class vAdicionando_Tb_Usuario_Espaco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GRIDHUB_USUARIOS",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    FotoPerfil = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRIDHUB_USUARIOS", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "GRIDHUB_ESPACOS",
                columns: table => new
                {
                    EspacoId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    NomeEspaco = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    FotoEspaco = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    FonteEnergia = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    OrientacaoSolar = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    MediaSolar = table.Column<double>(type: "BINARY_DOUBLE", precision: 18, scale: 2, nullable: false),
                    Topografia = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    AreaTotal = table.Column<double>(type: "BINARY_DOUBLE", precision: 18, scale: 2, nullable: false),
                    DirecaoVento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    VelocidadeVento = table.Column<double>(type: "BINARY_DOUBLE", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRIDHUB_ESPACOS", x => x.EspacoId);
                    table.ForeignKey(
                        name: "FK_GRIDHUB_ESPACOS_GRIDHUB_USUARIOS_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "GRIDHUB_USUARIOS",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GRIDHUB_ESPACOS_UsuarioId",
                table: "GRIDHUB_ESPACOS",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_GRIDHUB_USUARIOS_Email",
                table: "GRIDHUB_USUARIOS",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GRIDHUB_ESPACOS");

            migrationBuilder.DropTable(
                name: "GRIDHUB_USUARIOS");
        }
    }
}

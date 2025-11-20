using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dunder_Store.Migrations
{
    /// <inheritdoc />
    public partial class AddTabelaPrecoRegiao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrecoRegiao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Regiao = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecoBase = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecoRegiao", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // ==========================
            // SEED INICIAL DE REGIÕES
            // ==========================
            migrationBuilder.InsertData(
                table: "PrecoRegiao",
                columns: new[] { "Id", "Regiao", "PrecoBase" },
                values: new object[,]
                {
                    { 1, "Norte", 25.0m },
                    { 2, "Nordeste", 20.0m },
                    { 3, "Centro-Oeste", 18.0m },
                    { 4, "Sudeste", 12.0m },
                    { 5, "Sul", 15.0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrecoRegiao");
        }
    }
}

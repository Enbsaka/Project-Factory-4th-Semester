using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dunder_Store.Migrations
{
    /// <inheritdoc />
    public partial class AddFreteToPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FreteValor",
                table: "Pedidos",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreteValor",
                table: "Pedidos");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dunder_Store.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecoUnitarioPedidoProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId1",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_ClienteId1",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Pedidos");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "PedidoProdutos",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "PedidoProdutos");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId1",
                table: "Pedidos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteId1",
                table: "Pedidos",
                column: "ClienteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId1",
                table: "Pedidos",
                column: "ClienteId1",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}

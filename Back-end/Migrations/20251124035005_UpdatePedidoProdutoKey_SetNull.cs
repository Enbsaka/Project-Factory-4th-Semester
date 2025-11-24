using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dunder_Store.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePedidoProdutoKey_SetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET @fk1 := (SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE 
                             WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'PedidoProdutos' AND REFERENCED_TABLE_NAME = 'Produtos' LIMIT 1);
                SET @sql1 := IFNULL(@fk1, '');
                SET @stmt1 := IF(@sql1 <> '', CONCAT('ALTER TABLE `PedidoProdutos` DROP FOREIGN KEY `', @fk1, '`'), 'SELECT 1');
                PREPARE s1 FROM @stmt1; EXECUTE s1; DEALLOCATE PREPARE s1;

                SET @fk2 := (SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE 
                             WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'PedidoProdutos' AND REFERENCED_TABLE_NAME = 'Pedidos' LIMIT 1);
                SET @sql2 := IFNULL(@fk2, '');
                SET @stmt2 := IF(@sql2 <> '', CONCAT('ALTER TABLE `PedidoProdutos` DROP FOREIGN KEY `', @fk2, '`'), 'SELECT 1');
                PREPARE s2 FROM @stmt2; EXECUTE s2; DEALLOCATE PREPARE s2;
            ");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoProdutos",
                table: "PedidoProdutos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProdutoId",
                table: "PedidoProdutos",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.Sql("ALTER TABLE `PedidoProdutos` MODIFY COLUMN `ProdutoId` char(36) COLLATE ascii_general_ci NULL;");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoProdutos",
                table: "PedidoProdutos",
                column: "Id");


            migrationBuilder.AddForeignKey(
                name: "FK_PedidoProdutos_Pedidos_PedidoId",
                table: "PedidoProdutos",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoProdutos_Produtos_ProdutoId",
                table: "PedidoProdutos",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoProdutos_Produtos_ProdutoId",
                table: "PedidoProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoProdutos_Pedidos_PedidoId",
                table: "PedidoProdutos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoProdutos",
                table: "PedidoProdutos");


            migrationBuilder.AlterColumn<Guid>(
                name: "ProdutoId",
                table: "PedidoProdutos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoProdutos",
                table: "PedidoProdutos",
                columns: new[] { "PedidoId", "ProdutoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoProdutos_Pedidos_PedidoId",
                table: "PedidoProdutos",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoProdutos_Produtos_ProdutoId",
                table: "PedidoProdutos",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

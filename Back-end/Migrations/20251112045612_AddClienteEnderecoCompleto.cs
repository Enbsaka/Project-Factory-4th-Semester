using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dunder_Store.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteEnderecoCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // MySQL 5.7+: usa preparo dinâmico para adicionar colunas apenas se não existirem
            migrationBuilder.Sql(@"SET @c := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'Logradouro');
SET @stmt := IF(@c = 0, 'ALTER TABLE `Clientes` ADD COLUMN `Logradouro` varchar(255) NULL;', 'SELECT 1');
PREPARE p FROM @stmt; EXECUTE p; DEALLOCATE PREPARE p;");

            migrationBuilder.Sql(@"SET @c := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'Bairro');
SET @stmt := IF(@c = 0, 'ALTER TABLE `Clientes` ADD COLUMN `Bairro` varchar(255) NULL;', 'SELECT 1');
PREPARE p FROM @stmt; EXECUTE p; DEALLOCATE PREPARE p;");

            migrationBuilder.Sql(@"SET @c := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'Localidade');
SET @stmt := IF(@c = 0, 'ALTER TABLE `Clientes` ADD COLUMN `Localidade` varchar(255) NULL;', 'SELECT 1');
PREPARE p FROM @stmt; EXECUTE p; DEALLOCATE PREPARE p;");

            migrationBuilder.Sql(@"SET @c := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'Uf');
SET @stmt := IF(@c = 0, 'ALTER TABLE `Clientes` ADD COLUMN `Uf` varchar(10) NULL;', 'SELECT 1');
PREPARE p FROM @stmt; EXECUTE p; DEALLOCATE PREPARE p;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Localidade",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "Clientes");
        }
    }
}

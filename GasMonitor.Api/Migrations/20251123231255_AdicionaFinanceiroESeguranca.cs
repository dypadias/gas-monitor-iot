using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaFinanceiroESeguranca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoPago",
                table: "ProdutosConfig",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "VazamentoDetectado",
                table: "Medicoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoPago",
                table: "ProdutosConfig");

            migrationBuilder.DropColumn(
                name: "VazamentoDetectado",
                table: "Medicoes");
        }
    }
}

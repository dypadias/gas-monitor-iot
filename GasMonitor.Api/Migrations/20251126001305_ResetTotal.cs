using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GasMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class ResetTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoTrocas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataTroca = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrecoPago = table.Column<decimal>(type: "numeric", nullable: false),
                    NomeProduto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoTrocas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medicoes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDispositivo = table.Column<string>(type: "text", nullable: false),
                    PesoKg = table.Column<double>(type: "double precision", nullable: false),
                    DataHoraRegisto = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VazamentoDetectado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdutosConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    TaraKg = table.Column<double>(type: "double precision", nullable: false),
                    CapacidadeTotalKg = table.Column<double>(type: "double precision", nullable: false),
                    TipoUnidade = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    PrecoPago = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutosConfig", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoTrocas");

            migrationBuilder.DropTable(
                name: "Medicoes");

            migrationBuilder.DropTable(
                name: "ProdutosConfig");
        }
    }
}

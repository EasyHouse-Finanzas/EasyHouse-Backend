using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyHouse.Migrations
{
    /// <inheritdoc />
    public partial class AddAmortizationDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "HousingBonus",
                schema: "public",
                table: "configs",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateTable(
                name: "amortization_details",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    Payment = table.Column<decimal>(type: "numeric", nullable: false),
                    Interest = table.Column<decimal>(type: "numeric", nullable: false),
                    Amortization = table.Column<decimal>(type: "numeric", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amortization_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_amortization_details_simulations_SimulationId",
                        column: x => x.SimulationId,
                        principalSchema: "public",
                        principalTable: "simulations",
                        principalColumn: "SimulationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_amortization_details_SimulationId",
                schema: "public",
                table: "amortization_details",
                column: "SimulationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "amortization_details",
                schema: "public");

            migrationBuilder.AlterColumn<decimal>(
                name: "HousingBonus",
                schema: "public",
                table: "configs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}

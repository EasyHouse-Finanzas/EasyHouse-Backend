using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyHouse.Migrations
{
    /// <inheritdoc />
    public partial class AddSegurosAndGastosToDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Gastos",
                schema: "public",
                table: "amortization_details",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Seguros",
                schema: "public",
                table: "amortization_details",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gastos",
                schema: "public",
                table: "amortization_details");

            migrationBuilder.DropColumn(
                name: "Seguros",
                schema: "public",
                table: "amortization_details");
        }
    }
}

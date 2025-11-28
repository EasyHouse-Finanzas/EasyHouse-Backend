using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyHouse.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyAndPaymentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "public",
                table: "houses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                schema: "public",
                table: "amortization_details",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "public",
                table: "houses");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                schema: "public",
                table: "amortization_details");
        }
    }
}

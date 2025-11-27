using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyHouse.Migrations
{
    /// <inheritdoc />
    public partial class AddReportUrlToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportUrl",
                schema: "public",
                table: "reports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportUrl",
                schema: "public",
                table: "reports");
        }
    }
}

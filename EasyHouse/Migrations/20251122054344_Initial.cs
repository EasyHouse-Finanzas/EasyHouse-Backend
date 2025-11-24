using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyHouse.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "configs",
                schema: "public",
                columns: table => new
                {
                    ConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    RateType = table.Column<string>(type: "text", nullable: false),
                    Tea = table.Column<decimal>(type: "numeric", nullable: true),
                    Tna = table.Column<decimal>(type: "numeric", nullable: true),
                    Capitalization = table.Column<string>(type: "text", nullable: true),
                    GracePeriodType = table.Column<string>(type: "text", nullable: false),
                    GraceMonths = table.Column<int>(type: "integer", nullable: false),
                    HousingBonus = table.Column<decimal>(type: "numeric", nullable: false),
                    DisbursementCommission = table.Column<decimal>(type: "numeric", nullable: false),
                    MonthlyMaintenance = table.Column<decimal>(type: "numeric", nullable: false),
                    MonthlyFees = table.Column<decimal>(type: "numeric", nullable: false),
                    Itf = table.Column<decimal>(type: "numeric", nullable: false),
                    LifeInsurance = table.Column<decimal>(type: "numeric", nullable: false),
                    RiskInsurance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configs", x => x.ConfigId);
                });

            migrationBuilder.CreateTable(
                name: "houses",
                schema: "public",
                columns: table => new
                {
                    HouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Project = table.Column<string>(type: "text", nullable: false),
                    PropertyCode = table.Column<string>(type: "text", nullable: false),
                    TotalArea = table.Column<decimal>(type: "numeric", nullable: false),
                    BuiltArea = table.Column<decimal>(type: "numeric", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_houses", x => x.HouseId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                schema: "public",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DocumentNumber = table.Column<string>(type: "text", nullable: false),
                    Occupation = table.Column<string>(type: "text", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_clients_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "simulations",
                schema: "public",
                columns: table => new
                {
                    SimulationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    HouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    InitialQuota = table.Column<decimal>(type: "numeric", nullable: false),
                    TermMonths = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FixedQuota = table.Column<decimal>(type: "numeric", nullable: true),
                    TCEA = table.Column<decimal>(type: "numeric", nullable: true),
                    VAN = table.Column<decimal>(type: "numeric", nullable: true),
                    AnnualIRR = table.Column<decimal>(type: "numeric", nullable: true),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalInterests = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalCreditCost = table.Column<decimal>(type: "numeric", nullable: true),
                    DisbursementCommission = table.Column<decimal>(type: "numeric", nullable: true),
                    InsuranceMaintenanceFees = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simulations", x => x.SimulationId);
                    table.ForeignKey(
                        name: "FK_simulations_clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_simulations_configs_ConfigId",
                        column: x => x.ConfigId,
                        principalSchema: "public",
                        principalTable: "configs",
                        principalColumn: "ConfigId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_simulations_houses_HouseId",
                        column: x => x.HouseId,
                        principalSchema: "public",
                        principalTable: "houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                schema: "public",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_reports_simulations_SimulationId",
                        column: x => x.SimulationId,
                        principalSchema: "public",
                        principalTable: "simulations",
                        principalColumn: "SimulationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clients_UserId",
                schema: "public",
                table: "clients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_SimulationId",
                schema: "public",
                table: "reports",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_simulations_ClientId",
                schema: "public",
                table: "simulations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_simulations_ConfigId",
                schema: "public",
                table: "simulations",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_simulations_HouseId",
                schema: "public",
                table: "simulations",
                column: "HouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "simulations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "clients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "configs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "houses",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");
        }
    }
}

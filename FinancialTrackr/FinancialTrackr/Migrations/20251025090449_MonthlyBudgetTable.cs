using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTrackr.Migrations
{
    /// <inheritdoc />
    public partial class MonthlyBudgetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyBudget",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "MonthlyBudget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MonthlyBudgetAmount = table.Column<double>(type: "REAL", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyBudget_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudget_UserId",
                table: "MonthlyBudget",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyBudget");

            migrationBuilder.AddColumn<double>(
                name: "MonthlyBudget",
                table: "Users",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

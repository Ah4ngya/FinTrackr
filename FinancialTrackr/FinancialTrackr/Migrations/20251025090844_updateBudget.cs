using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTrackr.Migrations
{
    /// <inheritdoc />
    public partial class updateBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MonthlyBudget_UserId",
                table: "MonthlyBudget");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudget_UserId_Month",
                table: "MonthlyBudget",
                columns: new[] { "UserId", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MonthlyBudget_UserId_Month",
                table: "MonthlyBudget");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudget_UserId",
                table: "MonthlyBudget",
                column: "UserId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTrackr.Migrations
{
    /// <inheritdoc />
    public partial class updateBudgetAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyBudget_Users_UserId",
                table: "MonthlyBudget");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonthlyBudget",
                table: "MonthlyBudget");

            migrationBuilder.RenameTable(
                name: "MonthlyBudget",
                newName: "Budgets");

            migrationBuilder.RenameIndex(
                name: "IX_MonthlyBudget_UserId_Month",
                table: "Budgets",
                newName: "IX_Budgets_UserId_Month");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Users_UserId",
                table: "Budgets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Users_UserId",
                table: "Budgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.RenameTable(
                name: "Budgets",
                newName: "MonthlyBudget");

            migrationBuilder.RenameIndex(
                name: "IX_Budgets_UserId_Month",
                table: "MonthlyBudget",
                newName: "IX_MonthlyBudget_UserId_Month");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonthlyBudget",
                table: "MonthlyBudget",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyBudget_Users_UserId",
                table: "MonthlyBudget",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

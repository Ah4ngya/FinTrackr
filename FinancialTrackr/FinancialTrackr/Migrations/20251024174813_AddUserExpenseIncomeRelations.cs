using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTrackr.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExpenseIncomeRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Income",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Expenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Income_UserId",
                table: "Income",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_Users_UserId",
                table: "Income",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_Users_UserId",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Income_UserId",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Income");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Expenses");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetEntries_CategoryId",
                table: "BudgetEntries");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntries_CategoryId_Year_Month",
                table: "BudgetEntries",
                columns: new[] { "CategoryId", "Year", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetEntries_CategoryId_Year_Month",
                table: "BudgetEntries");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntries_CategoryId",
                table: "BudgetEntries",
                column: "CategoryId");
        }
    }
}

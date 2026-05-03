using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class TripLedgerUpdateaddtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseAmount",
                table: "TripLedgers");

            migrationBuilder.RenameColumn(
                name: "ExpenseName",
                table: "TripLedgers",
                newName: "TokenNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenNo",
                table: "TripLedgers",
                newName: "ExpenseName");

            migrationBuilder.AddColumn<decimal>(
                name: "ExpenseAmount",
                table: "TripLedgers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class TripLedgerUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripLedgers_TripEntries_TripEntryFkId",
                table: "TripLedgers");

            migrationBuilder.RenameColumn(
                name: "TripEntryFkId",
                table: "TripLedgers",
                newName: "TripEntryId");

            migrationBuilder.RenameColumn(
                name: "TotalExpense",
                table: "TripLedgers",
                newName: "ExpenseAmount");

            migrationBuilder.RenameIndex(
                name: "IX_TripLedgers_TripEntryFkId",
                table: "TripLedgers",
                newName: "IX_TripLedgers_TripEntryId");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseName",
                table: "TripLedgers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TripLedgers_TripEntries_TripEntryId",
                table: "TripLedgers",
                column: "TripEntryId",
                principalTable: "TripEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripLedgers_TripEntries_TripEntryId",
                table: "TripLedgers");

            migrationBuilder.DropColumn(
                name: "ExpenseName",
                table: "TripLedgers");

            migrationBuilder.RenameColumn(
                name: "TripEntryId",
                table: "TripLedgers",
                newName: "TripEntryFkId");

            migrationBuilder.RenameColumn(
                name: "ExpenseAmount",
                table: "TripLedgers",
                newName: "TotalExpense");

            migrationBuilder.RenameIndex(
                name: "IX_TripLedgers_TripEntryId",
                table: "TripLedgers",
                newName: "IX_TripLedgers_TripEntryFkId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripLedgers_TripEntries_TripEntryFkId",
                table: "TripLedgers",
                column: "TripEntryFkId",
                principalTable: "TripEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

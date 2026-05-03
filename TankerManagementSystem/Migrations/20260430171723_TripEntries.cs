using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankerManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class TripEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripLedgers_Tankers_TankerId",
                table: "TripLedgers");

            migrationBuilder.DropColumn(
                name: "FromLocation",
                table: "TripLedgers");

            migrationBuilder.DropColumn(
                name: "ToLocation",
                table: "TripLedgers");

            migrationBuilder.RenameColumn(
                name: "TankerId",
                table: "TripLedgers",
                newName: "TripEntryFkId");

            migrationBuilder.RenameIndex(
                name: "IX_TripLedgers_TankerId",
                table: "TripLedgers",
                newName: "IX_TripLedgers_TripEntryFkId");

            migrationBuilder.CreateTable(
                name: "TripEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TankerId = table.Column<int>(type: "int", nullable: false),
                    AdvanceCash = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FromLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripEntries_Tankers_TankerId",
                        column: x => x.TankerId,
                        principalTable: "Tankers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TripEntries_TankerId",
                table: "TripEntries",
                column: "TankerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripLedgers_TripEntries_TripEntryFkId",
                table: "TripLedgers",
                column: "TripEntryFkId",
                principalTable: "TripEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripLedgers_TripEntries_TripEntryFkId",
                table: "TripLedgers");

            migrationBuilder.DropTable(
                name: "TripEntries");

            migrationBuilder.RenameColumn(
                name: "TripEntryFkId",
                table: "TripLedgers",
                newName: "TankerId");

            migrationBuilder.RenameIndex(
                name: "IX_TripLedgers_TripEntryFkId",
                table: "TripLedgers",
                newName: "IX_TripLedgers_TankerId");

            migrationBuilder.AddColumn<string>(
                name: "FromLocation",
                table: "TripLedgers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToLocation",
                table: "TripLedgers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TripLedgers_Tankers_TankerId",
                table: "TripLedgers",
                column: "TankerId",
                principalTable: "Tankers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

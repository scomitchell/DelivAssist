using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GigBoardBackend.Migrations
{
    /// <inheritdoc />
    public partial class ShiftApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "App",
                table: "Shifts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "App",
                table: "Shifts");
        }
    }
}

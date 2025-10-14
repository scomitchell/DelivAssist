using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DelivAssist.Migrations
{
    /// <inheritdoc />
    public partial class mileage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Mileage",
                table: "Deliveries",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "Deliveries");
        }
    }
}

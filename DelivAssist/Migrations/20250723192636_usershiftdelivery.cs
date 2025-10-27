using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DelivAssist.Migrations
{
    /// <inheritdoc />
    public partial class UserShiftDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftDeliveries",
                table: "ShiftDeliveries");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ShiftDeliveries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftDeliveries",
                table: "ShiftDeliveries",
                columns: new[] { "ShiftId", "UserId", "DeliveryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftDeliveries_UserId",
                table: "ShiftDeliveries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftDeliveries_Users_UserId",
                table: "ShiftDeliveries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftDeliveries_Users_UserId",
                table: "ShiftDeliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShiftDeliveries",
                table: "ShiftDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_ShiftDeliveries_UserId",
                table: "ShiftDeliveries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShiftDeliveries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShiftDeliveries",
                table: "ShiftDeliveries",
                columns: new[] { "ShiftId", "DeliveryId" });
        }
    }
}

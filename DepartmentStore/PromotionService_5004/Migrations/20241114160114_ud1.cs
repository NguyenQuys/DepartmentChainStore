using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromotionService_5004.Migrations
{
    /// <inheritdoc />
    public partial class ud1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ApplyFor",
                table: "Promions",
                type: "int",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "ApplyFor",
                table: "Promions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}

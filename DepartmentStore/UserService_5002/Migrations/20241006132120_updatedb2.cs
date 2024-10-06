using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService_5002.Migrations
{
    /// <inheritdoc />
    public partial class updatedb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "OtherInfo");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "OtherInfo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

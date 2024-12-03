using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService_5002.Migrations
{
    /// <inheritdoc />
    public partial class up1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOtherInfo_Role_RoleId",
                table: "UserOtherInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOtherInfo_Role_RoleId",
                table: "UserOtherInfo",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOtherInfo_Role_RoleId",
                table: "UserOtherInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOtherInfo_Role_RoleId",
                table: "UserOtherInfo",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

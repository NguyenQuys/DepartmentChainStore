using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService_5002.Migrations
{
    /// <inheritdoc />
    public partial class updatedb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherInfo_Role_RoleId",
                table: "OtherInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_OtherInfo_Users_UserId",
                table: "OtherInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtherInfo",
                table: "OtherInfo");

            migrationBuilder.RenameTable(
                name: "OtherInfo",
                newName: "UserOtherInfo");

            migrationBuilder.RenameIndex(
                name: "IX_OtherInfo_RoleId",
                table: "UserOtherInfo",
                newName: "IX_UserOtherInfo_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserOtherInfo",
                table: "UserOtherInfo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOtherInfo_Role_RoleId",
                table: "UserOtherInfo",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOtherInfo_Users_UserId",
                table: "UserOtherInfo",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOtherInfo_Role_RoleId",
                table: "UserOtherInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOtherInfo_Users_UserId",
                table: "UserOtherInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserOtherInfo",
                table: "UserOtherInfo");

            migrationBuilder.RenameTable(
                name: "UserOtherInfo",
                newName: "OtherInfo");

            migrationBuilder.RenameIndex(
                name: "IX_UserOtherInfo_RoleId",
                table: "OtherInfo",
                newName: "IX_OtherInfo_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtherInfo",
                table: "OtherInfo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherInfo_Role_RoleId",
                table: "OtherInfo",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OtherInfo_Users_UserId",
                table: "OtherInfo",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService_5002.Migrations
{
    /// <inheritdoc />
    public partial class uodatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "OtherInfo",
                newName: "RoleId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "OtherInfo",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtherInfo_RoleId",
                table: "OtherInfo",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherInfo_Role_RoleId",
                table: "OtherInfo",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherInfo_Role_RoleId",
                table: "OtherInfo");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_OtherInfo_RoleId",
                table: "OtherInfo");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "OtherInfo",
                newName: "Role");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "OtherInfo",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}

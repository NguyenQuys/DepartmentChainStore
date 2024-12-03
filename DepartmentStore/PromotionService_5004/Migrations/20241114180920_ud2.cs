using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromotionService_5004.Migrations
{
    /// <inheritdoc />
    public partial class ud2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promions_PromotionTypes_IdPromotionType",
                table: "Promions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Promions",
                table: "Promions");

            migrationBuilder.RenameTable(
                name: "Promions",
                newName: "Promotions");

            migrationBuilder.RenameIndex(
                name: "IX_Promions_IdPromotionType",
                table: "Promotions",
                newName: "IX_Promotions_IdPromotionType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Promotions",
                table: "Promotions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_PromotionTypes_IdPromotionType",
                table: "Promotions",
                column: "IdPromotionType",
                principalTable: "PromotionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_PromotionTypes_IdPromotionType",
                table: "Promotions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Promotions",
                table: "Promotions");

            migrationBuilder.RenameTable(
                name: "Promotions",
                newName: "Promions");

            migrationBuilder.RenameIndex(
                name: "IX_Promotions_IdPromotionType",
                table: "Promions",
                newName: "IX_Promions_IdPromotionType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Promions",
                table: "Promions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Promions_PromotionTypes_IdPromotionType",
                table: "Promions",
                column: "IdPromotionType",
                principalTable: "PromotionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

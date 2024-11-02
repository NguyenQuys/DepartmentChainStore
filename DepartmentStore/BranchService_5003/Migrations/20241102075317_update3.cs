using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BranchService_5003.Migrations
{
    /// <inheritdoc />
    public partial class update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportProductHistories_Branches_IdBranch",
                table: "ExportProductHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExportProductHistories",
                table: "ExportProductHistories");

            migrationBuilder.RenameTable(
                name: "ExportProductHistories",
                newName: "ImportProductHistories");

            migrationBuilder.RenameIndex(
                name: "IX_ExportProductHistories_IdBranch",
                table: "ImportProductHistories",
                newName: "IX_ImportProductHistories_IdBranch");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportProductHistories",
                table: "ImportProductHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportProductHistories_Branches_IdBranch",
                table: "ImportProductHistories",
                column: "IdBranch",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportProductHistories_Branches_IdBranch",
                table: "ImportProductHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportProductHistories",
                table: "ImportProductHistories");

            migrationBuilder.RenameTable(
                name: "ImportProductHistories",
                newName: "ExportProductHistories");

            migrationBuilder.RenameIndex(
                name: "IX_ImportProductHistories_IdBranch",
                table: "ExportProductHistories",
                newName: "IX_ExportProductHistories_IdBranch");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExportProductHistories",
                table: "ExportProductHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportProductHistories_Branches_IdBranch",
                table: "ExportProductHistories",
                column: "IdBranch",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

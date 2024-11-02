using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BranchService_5003.Migrations
{
    /// <inheritdoc />
    public partial class update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "ExportProductHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBranch = table.Column<int>(type: "int", nullable: false),
                    IdProduct = table.Column<int>(type: "int", nullable: false),
                    IdBatch = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false),
                    Consignee = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ImportTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportProductHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportProductHistories_Branches_IdBranch",
                        column: x => x.IdBranch,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            

            migrationBuilder.CreateIndex(
                name: "IX_ExportProductHistories_IdBranch",
                table: "ExportProductHistories",
                column: "IdBranch");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportProductHistories");

          
        }
    }
}

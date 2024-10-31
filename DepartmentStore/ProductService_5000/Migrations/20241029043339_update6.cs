using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService_5000.Migrations
{
    /// <inheritdoc />
    public partial class update6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdReceive",
                table: "Batches");

            migrationBuilder.AddColumn<string>(
                name: "Receiver",
                table: "Batches",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Receiver",
                table: "Batches");

            migrationBuilder.AddColumn<int>(
                name: "IdReceive",
                table: "Batches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

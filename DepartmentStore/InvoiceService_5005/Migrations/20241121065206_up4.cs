using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceService_5005.Migrations
{
    /// <inheritdoc />
    public partial class up4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "CustomerNote",
                table: "Invoices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreNote",
                table: "Invoices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNote",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "StoreNote",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

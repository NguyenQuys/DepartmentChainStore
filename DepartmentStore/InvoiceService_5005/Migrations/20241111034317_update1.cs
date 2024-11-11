using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceService_5005.Migrations
{
    /// <inheritdoc />
    public partial class update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUserPurchase",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhoneNumber",
                table: "Invoices",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPhoneNumber",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "IdUserPurchase",
                table: "Invoices",
                type: "int",
                nullable: true);
        }
    }
}

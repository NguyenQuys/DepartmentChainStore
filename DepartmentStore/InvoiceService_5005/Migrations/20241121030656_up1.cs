using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceService_5005.Migrations
{
    /// <inheritdoc />
    public partial class up1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdEmployeeShip",
                table: "Invoices",
                newName: "EmployeeShip");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeShip",
                table: "Invoices",
                newName: "IdEmployeeShip");
        }
    }
}

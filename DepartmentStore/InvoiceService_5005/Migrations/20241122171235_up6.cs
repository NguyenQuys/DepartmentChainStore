using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceService_5005.Migrations
{
    /// <inheritdoc />
    public partial class up6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeShip",
                table: "Invoices",
                newName: "IdEmployeeShip");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdEmployeeShip",
                table: "Invoices",
                newName: "EmployeeShip");
        }
    }
}

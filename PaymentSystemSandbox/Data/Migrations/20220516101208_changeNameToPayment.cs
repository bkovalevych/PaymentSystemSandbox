using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentSystemSandbox.Data.Migrations
{
    public partial class changeNameToPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("PaymentTransactions", newName: "Payments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("Payments", newName: "PaymentTransactions");
        }
    }
}

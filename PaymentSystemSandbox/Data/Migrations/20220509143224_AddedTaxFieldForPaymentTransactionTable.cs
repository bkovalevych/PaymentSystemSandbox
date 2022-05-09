using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentSystemSandbox.Data.Migrations
{
    public partial class AddedTaxFieldForPaymentTransactionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceWithTax",
                table: "PaymentTransactions",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxInPercent",
                table: "PaymentTransactions",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceWithTax",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "TaxInPercent",
                table: "PaymentTransactions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBookingSystemDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationOnPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropCheckConstraint(
            //    name: "CK_Payment_PaymentMethod",
            //    table: "Payments");

            //migrationBuilder.DropCheckConstraint(
            //    name: "CK_Payment_Status",
            //    table: "Payments");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_PaymentMethod",
                table: "Payments",
                sql: "\"PaymentMethod\" IN ('Manual', 'Card')");


            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_Status",
                table: "Payments",
                sql: "\"Status\" IN ('Success', 'Failed')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Payment_PaymentMethod",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payment_Status",
                table: "Payments");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_PaymentMethod",
                table: "Payments",
                sql: "PaymentMethod IN ('Manual', 'Card')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_Status",
                table: "Payments",
                sql: "Status IN ('Success', 'Failed')");
        }

    }
}

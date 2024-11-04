using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readery.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPaybalAndStripePropertiesToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayPalPaymentDate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PayPalRefundId",
                table: "Orders",
                newName: "SessionId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Orders",
                newName: "PayPalRefundId");

            migrationBuilder.AddColumn<DateTime>(
                name: "PayPalPaymentDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }
    }
}

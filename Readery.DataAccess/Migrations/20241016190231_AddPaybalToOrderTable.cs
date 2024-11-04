using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readery.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPaybalToOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "Orders",
                newName: "PayPalRefundId");

            migrationBuilder.AddColumn<string>(
                name: "PayPalCaptureId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayPalOrderId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PayPalPaymentDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayPalCaptureId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PayPalOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PayPalPaymentDate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PayPalRefundId",
                table: "Orders",
                newName: "PaymentIntentId");
        }
    }
}

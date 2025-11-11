using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Migrations
{
    /// <inheritdoc />
    public partial class CorrectionSalesAndReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Products_ProductId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_ClientId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_ProductId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "IVATotal",
                table: "Receipts",
                newName: "IvaTotal");

            migrationBuilder.RenameColumn(
                name: "SaleId",
                table: "Receipts",
                newName: "ClientId");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Sales",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerUnit",
                table: "Sales",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiptDate",
                table: "Receipts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ProductId",
                table: "Sales",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ClientId",
                table: "Receipts",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_AspNetUsers_ClientId",
                table: "Receipts",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_ClientId",
                table: "Sales",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Products_ProductId",
                table: "Sales",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_AspNetUsers_ClientId",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_ClientId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Products_ProductId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ProductId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_ClientId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PricePerUnit",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ReceiptDate",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "IvaTotal",
                table: "Receipts",
                newName: "IVATotal");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Receipts",
                newName: "SaleId");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Receipts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_ProductId",
                table: "Receipts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Products_ProductId",
                table: "Receipts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_ClientId",
                table: "Sales",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

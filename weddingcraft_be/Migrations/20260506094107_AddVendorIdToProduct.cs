using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace weddingcraft_be.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorIdToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    PriceRange = table.Column<string>(type: "text", nullable: true),
                    BusinessName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_VendorProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendorId",
                table: "Products",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VendorId",
                table: "Orders",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_VendorId",
                table: "Orders",
                column: "VendorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_VendorId",
                table: "Products",
                column: "VendorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_VendorId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_VendorId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "VendorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Products_VendorId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_VendorId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}

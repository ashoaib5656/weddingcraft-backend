using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace weddingcraft_be.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToVendorProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "VendorProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "VendorProfiles");
        }
    }
}

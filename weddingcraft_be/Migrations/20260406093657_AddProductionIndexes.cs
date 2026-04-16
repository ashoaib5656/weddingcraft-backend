using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace weddingcraft_be.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Timestamp",
                schema: "public",
                table: "logs",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_CreatedAt",
                schema: "public",
                table: "chat_messages",
                column: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Logs_Timestamp",
                schema: "public",
                table: "logs");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_CreatedAt",
                schema: "public",
                table: "chat_messages");
        }
    }
}

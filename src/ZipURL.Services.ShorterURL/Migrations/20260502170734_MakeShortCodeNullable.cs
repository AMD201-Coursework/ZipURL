using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZipURL.Services.ShorterURL.Migrations
{
    /// <inheritdoc />
    public partial class MakeShortCodeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_URLItems_ShortCode",
                table: "URLItems");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "URLItems",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_URLItems_ShortCode",
                table: "URLItems",
                column: "ShortCode",
                unique: true,
                filter: "[ShortCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_URLItems_ShortCode",
                table: "URLItems");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "URLItems",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_URLItems_ShortCode",
                table: "URLItems",
                column: "ShortCode",
                unique: true);
        }
    }
}

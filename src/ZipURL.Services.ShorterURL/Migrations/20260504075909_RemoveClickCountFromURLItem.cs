using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZipURL.Services.ShorterURL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClickCountFromURLItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "URLItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "URLItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

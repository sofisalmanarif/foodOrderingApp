using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodOrderingApp.Migrations
{
    /// <inheritdoc />
    public partial class isAvlaibaleAddedInMenuItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "MenuItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "MenuItems");
        }
    }
}

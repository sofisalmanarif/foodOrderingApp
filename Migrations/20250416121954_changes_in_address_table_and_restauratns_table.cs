using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodOrderingApp.Migrations
{
    /// <inheritdoc />
    public partial class changes_in_address_table_and_restauratns_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidDocument",
                table: "Restaurants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidDocument",
                table: "Restaurants");
        }
    }
}

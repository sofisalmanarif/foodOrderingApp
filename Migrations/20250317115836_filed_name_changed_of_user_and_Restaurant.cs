using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodOrderingApp.Migrations
{
    /// <inheritdoc />
    public partial class filed_name_changed_of_user_and_Restaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Restaurants",
                newName: "RestaurantPhone");

            migrationBuilder.AddColumn<string>(
                name: "RestaurantName",
                table: "Restaurants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestaurantName",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "RestaurantPhone",
                table: "Restaurants",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Restaurants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

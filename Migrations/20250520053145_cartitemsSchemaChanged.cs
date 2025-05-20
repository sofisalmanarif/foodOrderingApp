using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodOrderingApp.Migrations
{
    /// <inheritdoc />
    public partial class cartitemsSchemaChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                table: "CartItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ItemId",
                table: "CartItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_MenuItemVarients_VariantId",
                table: "CartItems",
                column: "VariantId",
                principalTable: "MenuItemVarients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_MenuItems_ItemId",
                table: "CartItems",
                column: "ItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_MenuItemVarients_VariantId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_MenuItems_ItemId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ItemId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "CartItems");
        }
    }
}

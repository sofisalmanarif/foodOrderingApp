using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace foodOrderingApp.Migrations
{
    /// <inheritdoc />
    public partial class fewchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Address_AddressId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AddressId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "RefId",
                table: "Address",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Address_RefId",
                table: "Address",
                column: "RefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Users_RefId",
                table: "Address",
                column: "RefId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Users_RefId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_RefId",
                table: "Address");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RefId",
                table: "Address",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Address_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzMap.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema_EzMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Poi_User_UserId",
                table: "Poi");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Poi",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Poi_User_UserId",
                table: "Poi",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Poi_User_UserId",
                table: "Poi");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Poi",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Poi_User_UserId",
                table: "Poi",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

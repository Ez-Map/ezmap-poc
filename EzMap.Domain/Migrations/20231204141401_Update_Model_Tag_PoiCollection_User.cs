using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzMap.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Update_Model_Tag_PoiCollection_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Tag",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PoiCollection",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tag_UserId",
                table: "Tag",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PoiCollection_UserId",
                table: "PoiCollection",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PoiCollection_User_UserId",
                table: "PoiCollection",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_User_UserId",
                table: "Tag",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoiCollection_User_UserId",
                table: "PoiCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_User_UserId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_UserId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_PoiCollection_UserId",
                table: "PoiCollection");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PoiCollection");
        }
    }
}

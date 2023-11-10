using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzMap.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Poi_Collection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Poi");

            migrationBuilder.CreateTable(
                name: "PoiCollection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoiCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoiPoiCollection",
                columns: table => new
                {
                    PoiCollectionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoiPoiCollection", x => new { x.PoiCollectionsId, x.PoisId });
                    table.ForeignKey(
                        name: "FK_PoiPoiCollection_PoiCollection_PoiCollectionsId",
                        column: x => x.PoiCollectionsId,
                        principalTable: "PoiCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoiPoiCollection_Poi_PoisId",
                        column: x => x.PoisId,
                        principalTable: "Poi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoiPoiCollection_PoisId",
                table: "PoiPoiCollection",
                column: "PoisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoiPoiCollection");

            migrationBuilder.DropTable(
                name: "PoiCollection");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Poi",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}

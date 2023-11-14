using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EzMap.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PoiCollecTion_DefaultViewType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultViewType",
                table: "PoiCollection",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultViewType",
                table: "PoiCollection");
        }
    }
}

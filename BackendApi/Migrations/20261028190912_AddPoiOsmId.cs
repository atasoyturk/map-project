using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPoiOsmId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OsmId",
                table: "tbl_poi",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_poi_OsmId",
                table: "tbl_poi",
                column: "OsmId",
                unique: true,
                filter: "\"OsmId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_poi_OsmId",
                table: "tbl_poi");

            migrationBuilder.DropColumn(
                name: "OsmId",
                table: "tbl_poi");
        }
    }
}

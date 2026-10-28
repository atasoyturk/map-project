using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class EnforceSrid4326AndGistIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rows written via GeometryConverter.FromWkt had SRID 0; tag them 4326 before the typed columns reject them.
            migrationBuilder.Sql("""
                UPDATE tbl_transit_stop   SET "Geometry"         = ST_SetSRID("Geometry", 4326)         WHERE ST_SRID("Geometry") = 0;
                UPDATE tbl_transit_route  SET "RouteGeometry"    = ST_SetSRID("RouteGeometry", 4326)    WHERE ST_SRID("RouteGeometry") = 0;
                UPDATE tbl_polygon        SET "Geometry"         = ST_SetSRID("Geometry", 4326)         WHERE ST_SRID("Geometry") = 0;
                UPDATE tbl_point          SET "Geometry"         = ST_SetSRID("Geometry", 4326)         WHERE ST_SRID("Geometry") = 0;
                UPDATE tbl_poi            SET "Geometry"         = ST_SetSRID("Geometry", 4326)         WHERE ST_SRID("Geometry") = 0;
                UPDATE tbl_line           SET "Geometry"         = ST_SetSRID("Geometry", 4326)         WHERE ST_SRID("Geometry") = 0;
                UPDATE tbl_geo_permission SET "BoundaryGeometry" = ST_SetSRID("BoundaryGeometry", 4326) WHERE ST_SRID("BoundaryGeometry") = 0;
                UPDATE tbl_annotation     SET "Geometry"         = ST_SetSRID("Geometry", 4326)         WHERE ST_SRID("Geometry") = 0;
                """);

            migrationBuilder.AlterColumn<Point>(
                name: "Geometry",
                table: "tbl_transit_stop",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry");

            migrationBuilder.AlterColumn<Geometry>(
                name: "RouteGeometry",
                table: "tbl_transit_route",
                type: "geometry(Geometry,4326)",
                nullable: true,
                oldClrType: typeof(Geometry),
                oldType: "geometry",
                oldNullable: true);

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_polygon",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_point",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");

            migrationBuilder.AlterColumn<Point>(
                name: "Geometry",
                table: "tbl_poi",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_line",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");

            migrationBuilder.AlterColumn<Geometry>(
                name: "BoundaryGeometry",
                table: "tbl_geo_permission",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_annotation",
                type: "geometry(Geometry,4326)",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_polygon_Geometry",
                table: "tbl_polygon",
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_point_Geometry",
                table: "tbl_point",
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_poi_Geometry",
                table: "tbl_poi",
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_line_Geometry",
                table: "tbl_line",
                column: "Geometry")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_polygon_Geometry",
                table: "tbl_polygon");

            migrationBuilder.DropIndex(
                name: "IX_tbl_point_Geometry",
                table: "tbl_point");

            migrationBuilder.DropIndex(
                name: "IX_tbl_poi_Geometry",
                table: "tbl_poi");

            migrationBuilder.DropIndex(
                name: "IX_tbl_line_Geometry",
                table: "tbl_line");

            migrationBuilder.AlterColumn<Point>(
                name: "Geometry",
                table: "tbl_transit_stop",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry(Geometry,4326)");

            migrationBuilder.AlterColumn<Geometry>(
                name: "RouteGeometry",
                table: "tbl_transit_route",
                type: "geometry",
                nullable: true,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Geometry,4326)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_polygon",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Geometry,4326)");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_point",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Geometry,4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Geometry",
                table: "tbl_poi",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry(Geometry,4326)");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_line",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Geometry,4326)");

            migrationBuilder.AlterColumn<Geometry>(
                name: "BoundaryGeometry",
                table: "tbl_geo_permission",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Geometry,4326)");

            migrationBuilder.AlterColumn<Geometry>(
                name: "Geometry",
                table: "tbl_annotation",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Geometry),
                oldType: "geometry(Geometry,4326)");
        }
    }
}

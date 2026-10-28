using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGeometryValidityChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Abort instead of silently replacing a degenerate shape with an empty geometry; such rows need a manual decision.
            migrationBuilder.Sql("""
                DO $$
                DECLARE lost text;
                BEGIN
                    SELECT string_agg(t || ':' || id, ', ') INTO lost FROM (
                        SELECT 'tbl_polygon' AS t, "Id" AS id FROM tbl_polygon
                            WHERE NOT ST_IsValid("Geometry") AND ST_IsEmpty(ST_CollectionExtract(ST_MakeValid("Geometry"), 3))
                        UNION ALL SELECT 'tbl_geo_permission', "Id" FROM tbl_geo_permission
                            WHERE NOT ST_IsValid("BoundaryGeometry") AND ST_IsEmpty(ST_CollectionExtract(ST_MakeValid("BoundaryGeometry"), 3))
                        UNION ALL SELECT 'tbl_line', "Id" FROM tbl_line
                            WHERE NOT ST_IsValid("Geometry") AND ST_IsEmpty(ST_CollectionExtract(ST_MakeValid("Geometry"), 2))
                        UNION ALL SELECT 'tbl_annotation', "Id" FROM tbl_annotation
                            WHERE NOT ST_IsValid("Geometry") AND ST_IsEmpty(ST_CollectionExtract(ST_MakeValid("Geometry"), ST_Dimension("Geometry") + 1))
                    ) x;
                    IF lost IS NOT NULL THEN
                        RAISE EXCEPTION 'Invalid geometries would become empty after ST_MakeValid: %', lost;
                    END IF;
                END $$;
                """);

            // Repair existing invalid rows; CollectionExtract keeps the original dimension so no GeometryCollection is stored.
            // Annotations can hold any type: ST_Dimension + 1 keeps the highest-dimension parts (lower ones are dropped).
            migrationBuilder.Sql("""
                UPDATE tbl_polygon        SET "Geometry"         = ST_CollectionExtract(ST_MakeValid("Geometry"), 3)         WHERE NOT ST_IsValid("Geometry");
                UPDATE tbl_geo_permission SET "BoundaryGeometry" = ST_CollectionExtract(ST_MakeValid("BoundaryGeometry"), 3) WHERE NOT ST_IsValid("BoundaryGeometry");
                UPDATE tbl_line           SET "Geometry"         = ST_CollectionExtract(ST_MakeValid("Geometry"), 2)         WHERE NOT ST_IsValid("Geometry");
                UPDATE tbl_annotation     SET "Geometry"         = ST_CollectionExtract(ST_MakeValid("Geometry"), ST_Dimension("Geometry") + 1) WHERE NOT ST_IsValid("Geometry");
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_polygon_Geometry_Valid",
                table: "tbl_polygon",
                sql: "ST_IsValid(\"Geometry\")");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_line_Geometry_Valid",
                table: "tbl_line",
                sql: "ST_IsValid(\"Geometry\")");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_geo_permission_BoundaryGeometry_Valid",
                table: "tbl_geo_permission",
                sql: "ST_IsValid(\"BoundaryGeometry\")");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_annotation_Geometry_Valid",
                table: "tbl_annotation",
                sql: "ST_IsValid(\"Geometry\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_polygon_Geometry_Valid",
                table: "tbl_polygon");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_line_Geometry_Valid",
                table: "tbl_line");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_geo_permission_BoundaryGeometry_Valid",
                table: "tbl_geo_permission");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_annotation_Geometry_Valid",
                table: "tbl_annotation");
        }
    }
}

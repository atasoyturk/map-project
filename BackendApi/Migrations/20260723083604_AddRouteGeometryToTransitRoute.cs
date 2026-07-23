using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteGeometryToTransitRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Geometry>(
                name: "RouteGeometry",
                table: "tbl_transit_route",
                type: "geometry",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8464), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8465) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8467), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8467) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8468), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8469) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8470), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8470) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8471), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8472) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8473), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8473) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8474), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8474) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8475), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8475) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8476), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8476) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8477), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8478) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8479), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8479) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8480), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8480) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8481), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8481) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8482), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8483) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8484), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8484) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8297), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8300) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8303), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8304) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8305), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8305) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8306), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8306) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8308), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8308) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8309), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8309) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8310), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8310) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8675), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8675) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8677), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8677) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8678), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8678) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8679), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8679) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8680), new DateTime(2026, 7, 23, 8, 36, 3, 964, DateTimeKind.Utc).AddTicks(8680) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteGeometry",
                table: "tbl_transit_route");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6696), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6696) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6700), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6701) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6703), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6703) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6705), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6706) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6708), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6708) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6710), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6711) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6713), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6713) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6715), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6715) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6717), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6717) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6719), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6719) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6721), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6722) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6724), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6724) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6782), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6783) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6785), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6786) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6788), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6788) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6280), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6284) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6290), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6290) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6292), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6292) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6294), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6295) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6297), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6297) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6299), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6300) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6301), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(6302) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7101), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7101) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7104), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7104) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7106), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7107) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7108), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7109) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7110), new DateTime(2026, 7, 21, 12, 0, 53, 378, DateTimeKind.Utc).AddTicks(7111) });
        }
    }
}

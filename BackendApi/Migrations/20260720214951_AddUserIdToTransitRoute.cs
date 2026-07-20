using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTransitRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tbl_transit_route",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3030), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3031) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3034), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3035) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3037), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3037) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3039), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3039) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3041), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3041) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3043), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3043) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3045), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3045) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3047), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3047) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3049), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3049) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3051), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3051) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3053), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3053) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3055), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3055) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2784), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2788) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2792), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2793) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2795), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2795) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2797), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2797) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2799), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2799) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2801), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2801) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2803), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(2803) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3296), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3296) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3298), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3298) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3300), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3300) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3302), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3302) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3303), new DateTime(2026, 7, 20, 21, 49, 50, 282, DateTimeKind.Utc).AddTicks(3304) });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_transit_route_UserId",
                table: "tbl_transit_route",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_transit_route_Users_UserId",
                table: "tbl_transit_route",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_transit_route_Users_UserId",
                table: "tbl_transit_route");

            migrationBuilder.DropIndex(
                name: "IX_tbl_transit_route_UserId",
                table: "tbl_transit_route");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_transit_route");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1963), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1963) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1965), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1965) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1967), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1967) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1968), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1968) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1969), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1969) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1995), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1995) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1997), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1997) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1998), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1998) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1999), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1999) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2000), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2000) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2001), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2001) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2002), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2003) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1832), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1834) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1837), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1838) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1839), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1839) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1840), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1840) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1841), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1841) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1842), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1843) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1843), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1844) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2117), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2118) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2119), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2120) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2120), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2121) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2122), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2122) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2122), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2123) });
        }
    }
}

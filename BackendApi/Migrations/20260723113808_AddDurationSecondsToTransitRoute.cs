using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationSecondsToTransitRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DurationSeconds",
                table: "tbl_transit_route",
                type: "double precision",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(622), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(622) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(625), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(626) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(627), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(628) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(629), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(629) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(631), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(631) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(632), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(633) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(634), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(634) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(636), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(636) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(637), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(638) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(639), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(639) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(640), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(641) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(642), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(642) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(644), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(644) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(645), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(646) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(647), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(647) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(425), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(429) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(433), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(433) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(435), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(435) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(436), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(437) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(438), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(438) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(439), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(440) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(441), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(441) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(863), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(863) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(865), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(866) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(867), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(867) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(869), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(869) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(870), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(870) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "tbl_transit_route");

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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTypeToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "tbl_vehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7366), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7367) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7369), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7369) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7371), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7371) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7372), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7372) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7373), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7374) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7375), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7375) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7376), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7376) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7378), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7378) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7379), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7379) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7380), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7380) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7381), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7381) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7382), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7382) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7383), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7384) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7385), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7385) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7386), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7386) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7074), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7077) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7082), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7082) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7083), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7083) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7084), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7085) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7086), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7086) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7087), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7088), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7088) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7556), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7556) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7557), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7558) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7559), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7559) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7560), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7560) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7561), new DateTime(2026, 7, 30, 13, 54, 9, 976, DateTimeKind.Utc).AddTicks(7561) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "tbl_vehicle");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4917), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4917) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4920), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4920) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4921), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4922) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4923), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4923) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4924), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4924) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4925), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4925) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4926), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4926) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4927), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4928) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4928), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4929) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4930), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4930) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4931), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4931) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4932), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4932) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4933), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4933) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4934), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4934) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4935), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4935) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4707), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4711) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4713), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4713) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4714), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4714) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4715), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4716) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4717), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4717) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4718), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4718) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4719), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(4719) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5099), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5100) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5101), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5101) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5102), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5102) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5103), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5103) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5104), new DateTime(2026, 7, 24, 8, 4, 50, 356, DateTimeKind.Utc).AddTicks(5104) });
        }
    }
}

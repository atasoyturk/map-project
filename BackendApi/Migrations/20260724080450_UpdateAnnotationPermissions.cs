using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnnotationPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, 2 });

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

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 6, 3 },
                    { 8, 3 },
                    { 6, 5 },
                    { 6, 7 },
                    { 8, 7 }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 7 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6262), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6262) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6267), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6267) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6269), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6269) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6271), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6271) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6273), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6273) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6274), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6274) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6276), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6276) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6277), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6277) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6279), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6279) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6280), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6281) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6282), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6282) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6283), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6283) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6285), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6285) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6286), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6287) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6288), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6288) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 5, 2 });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6076), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6078) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6082), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6082) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6084), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6084) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6085), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6086) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6087), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6089), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6089) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6090), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6090) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6505), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6506) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6508), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6508) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6510), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6511), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6512) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6513), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6513) });
        }
    }
}

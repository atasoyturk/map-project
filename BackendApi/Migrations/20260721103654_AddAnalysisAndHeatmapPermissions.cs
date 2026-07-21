using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisAndHeatmapPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2863), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2863) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2866), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2866) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2868), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2868) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2869), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2869) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2871), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2871) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2872), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2872) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2873), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2874) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2875), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2875) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2876), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2876) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2877), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2877) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2878), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2879) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2880), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2880) });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[,]
                {
                    { 13, new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2881), "Alan tarama analizi yapma yetkisi", true, false, new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2881), null, "area_scan" },
                    { 14, new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2882), "Konum analizi yapma yetkisi", true, false, new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2883), null, "location_analysis" },
                    { 15, new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2884), "Isı haritası görüntüleme yetkisi", true, false, new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2884), null, "heatmap_view" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 7, 2 },
                    { 8, 2 },
                    { 10, 2 },
                    { 11, 2 },
                    { 7, 4 },
                    { 8, 4 },
                    { 10, 4 },
                    { 11, 4 },
                    { 12, 4 }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2640), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2643) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2647), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2647) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2648), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2649) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2650), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2650) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2652), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2652) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2653), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2653) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2654), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(2654) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3034), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3035) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3036), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3036) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3037), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3038) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3038), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3039) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3078), new DateTime(2026, 7, 21, 10, 36, 54, 331, DateTimeKind.Utc).AddTicks(3079) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 13, 2 },
                    { 14, 2 },
                    { 15, 2 },
                    { 13, 4 },
                    { 14, 4 },
                    { 15, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, 4 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15);

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
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1788), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1788) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1792), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1792) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1794), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1795) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1796), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1797) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1798), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1799) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1800), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1801) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1802), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1803) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1804), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1805) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1806), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1807) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1570), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1572) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1577), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1577) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1579), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1580) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1581), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1582) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1584), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1584) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[] { 6, new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1586), true, false, new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1586), null, "Kullanıcı" });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2018), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2018) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2021), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2021) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2023), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2023) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2025), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2025) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2026), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(2027) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 8, 6 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 6 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6424), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6425) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6427), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6427) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6429), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6429) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6430), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6430) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6431), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6431) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6432), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6432) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6433), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6434) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6435), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6435) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6436), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6436) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6265), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6268) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6271), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6272) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6273), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6273) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6274), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6274) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6275), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6275) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6571), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6572) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6573), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6573) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6574), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6574) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6575), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6575) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6576), new DateTime(2026, 7, 14, 17, 48, 56, 373, DateTimeKind.Utc).AddTicks(6576) });
        }
    }
}

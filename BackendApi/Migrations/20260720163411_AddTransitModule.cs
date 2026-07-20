using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTransitModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_transit_route",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_transit_route", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_transit_stop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Geometry = table.Column<Point>(type: "geometry", nullable: false),
                    TransitRouteId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_transit_stop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_transit_stop_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_transit_stop_tbl_transit_route_TransitRouteId",
                        column: x => x.TransitRouteId,
                        principalTable: "tbl_transit_route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[,]
                {
                    { 10, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2000), "Durak oluşturma/güncelleme yetkisi", true, false, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2000), null, "transit_stop_create" },
                    { 11, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2001), "Durak/güzergah görüntüleme yetkisi", true, false, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2001), null, "transit_stop_read" },
                    { 12, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2002), "Güzergah yönetme yetkisi", true, false, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(2003), null, "transit_route_manage" }
                });

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
                columns: new[] { "CreatedDate", "ModifiedDate", "Name" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1841), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1841), "POI Operatorü" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1842), new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1843) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[] { 7, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1843), true, false, new DateTime(2026, 7, 20, 16, 34, 11, 39, DateTimeKind.Utc).AddTicks(1844), null, "Ulaşım Operatörü" });

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

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 11, 6 },
                    { 10, 7 },
                    { 11, 7 },
                    { 12, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_transit_stop_TransitRouteId",
                table: "tbl_transit_stop",
                column: "TransitRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_transit_stop_UserId",
                table: "tbl_transit_stop",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_transit_stop");

            migrationBuilder.DropTable(
                name: "tbl_transit_route");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 6 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, 7 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, 7 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7);

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
                columns: new[] { "CreatedDate", "ModifiedDate", "Name" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1584), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1584), "Operator" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1586), new DateTime(2026, 7, 17, 9, 35, 36, 800, DateTimeKind.Utc).AddTicks(1586) });

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
        }
    }
}

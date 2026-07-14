using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPoiModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_poi_category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_poi_category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_poi_category_tbl_poi_category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "tbl_poi_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_poi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    WorkingHours = table.Column<string>(type: "text", nullable: false),
                    Geometry = table.Column<Point>(type: "geometry", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_poi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_poi_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_poi_tbl_poi_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tbl_poi_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9386), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9386) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9389), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9389) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9390), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9390) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9391), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9391) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9393), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9393) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9394), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9394) });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[,]
                {
                    { 7, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9395), "POI oluşturma/güncelleme yetkisi", true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9395), null, "poi_create" },
                    { 8, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9396), "POI görüntüleme yetkisi", true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9396), null, "poi_read" },
                    { 9, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9398), "POI kategori ağacını yönetme yetkisi", true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9398), null, "poi_category_manage" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9220), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9224) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9227), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9227) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9228), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9228) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9229), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9229) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[] { 5, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9230), true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9230), null, "Operator" });

            migrationBuilder.InsertData(
                table: "tbl_poi_category",
                columns: new[] { "Id", "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9495), true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9495), null, "Yeme İçme", null },
                    { 4, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9499), true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9499), null, "Turizm", null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 7, 5 },
                    { 8, 5 }
                });

            migrationBuilder.InsertData(
                table: "tbl_poi_category",
                columns: new[] { "Id", "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9496), true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9496), null, "Restoran", 1 },
                    { 3, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9497), true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9498), null, "Kafe", 1 },
                    { 5, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9499), true, false, new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9500), null, "Müze", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_poi_CategoryId",
                table: "tbl_poi",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_poi_UserId",
                table: "tbl_poi",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_poi_category_ParentCategoryId",
                table: "tbl_poi_category",
                column: "ParentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_poi");

            migrationBuilder.DropTable(
                name: "tbl_poi_category");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, 5 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9694), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9694) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9698), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9698) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9700), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9700) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9702), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9702) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9704), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9705) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9706), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9706) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9494), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9497) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9501), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9501) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9503), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9504) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9505), new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9506) });
        }
    }
}

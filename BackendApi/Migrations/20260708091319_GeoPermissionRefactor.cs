using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class GeoPermissionRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_geo_permission_Roles_RoleId",
                table: "tbl_geo_permission");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_geo_permission_Users_UserId",
                table: "tbl_geo_permission");

            migrationBuilder.DropIndex(
                name: "IX_tbl_geo_permission_RoleId",
                table: "tbl_geo_permission");

            migrationBuilder.DropIndex(
                name: "IX_tbl_geo_permission_UserId",
                table: "tbl_geo_permission");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "tbl_geo_permission");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tbl_geo_permission");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "tbl_geo_permission",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "tbl_role_geo_permission",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    GeoPermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role_geo_permission", x => new { x.RoleId, x.GeoPermissionId });
                    table.ForeignKey(
                        name: "FK_tbl_role_geo_permission_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_role_geo_permission_tbl_geo_permission_GeoPermissionId",
                        column: x => x.GeoPermissionId,
                        principalTable: "tbl_geo_permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_geo_permission",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GeoPermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_geo_permission", x => new { x.UserId, x.GeoPermissionId });
                    table.ForeignKey(
                        name: "FK_tbl_user_geo_permission_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_user_geo_permission_tbl_geo_permission_GeoPermissionId",
                        column: x => x.GeoPermissionId,
                        principalTable: "tbl_geo_permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(766), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(767) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(769), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(769) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(770), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(771) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(772), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(772) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(646), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(648) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(651), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(651) });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_geo_permission_GeoPermissionId",
                table: "tbl_role_geo_permission",
                column: "GeoPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_geo_permission_GeoPermissionId",
                table: "tbl_user_geo_permission",
                column: "GeoPermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_role_geo_permission");

            migrationBuilder.DropTable(
                name: "tbl_user_geo_permission");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "tbl_geo_permission");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "tbl_geo_permission",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tbl_geo_permission",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7320), new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7320) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7323), new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7323) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7324), new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7324) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7325), new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7325) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7186), new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7188) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7191), new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7191) });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_geo_permission_RoleId",
                table: "tbl_geo_permission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_geo_permission_UserId",
                table: "tbl_geo_permission",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_geo_permission_Roles_RoleId",
                table: "tbl_geo_permission",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_geo_permission_Users_UserId",
                table: "tbl_geo_permission",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

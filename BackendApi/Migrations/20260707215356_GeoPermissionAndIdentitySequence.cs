using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class GeoPermissionAndIdentitySequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModifiedUserId",
                table: "Roles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModifiedUserId",
                table: "Permissions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tbl_geo_permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    BoundaryGeometry = table.Column<Geometry>(type: "geometry", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_geo_permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_geo_permission_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tbl_geo_permission_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7320), true, false, new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7320), null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7323), true, false, new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7323), null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7324), true, false, new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7324), null });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7325), true, false, new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7325), null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7186), true, false, new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7188), null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId" },
                values: new object[] { new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7191), true, false, new DateTime(2026, 7, 7, 21, 53, 55, 937, DateTimeKind.Utc).AddTicks(7191), null });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_geo_permission_RoleId",
                table: "tbl_geo_permission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_geo_permission_UserId",
                table: "tbl_geo_permission",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_geo_permission");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "Permissions");
        }
    }
}

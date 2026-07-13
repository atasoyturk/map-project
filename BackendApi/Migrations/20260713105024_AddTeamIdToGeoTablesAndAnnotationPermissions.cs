using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamIdToGeoTablesAndAnnotationPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "tbl_polygon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "tbl_point",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "tbl_line",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""tbl_point"" p
                SET ""TeamId"" = u.""TeamId""
                FROM ""Users"" u
                WHERE p.""UserId"" = u.""Id"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""tbl_line"" l
                SET ""TeamId"" = u.""TeamId""
                FROM ""Users"" u
                WHERE l.""UserId"" = u.""Id"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""tbl_polygon"" pg
                SET ""TeamId"" = u.""TeamId""
                FROM ""Users"" u
                WHERE pg.""UserId"" = u.""Id"";
            ");

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

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[,]
                {
                    { 5, new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9704), "Not/işaret ekleme yetkisi", true, false, new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9705), null, "annotation_create" },
                    { 6, new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9706), "Not geçmişini görüntüleme yetkisi", true, false, new DateTime(2026, 7, 13, 10, 50, 23, 877, DateTimeKind.Utc).AddTicks(9706), null, "annotation_read" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 3 },
                    { 2, 3 },
                    { 3, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 3, 4 }
                });

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

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 5, 1 },
                    { 6, 1 },
                    { 5, 2 },
                    { 6, 2 },
                    { 5, 3 },
                    { 5, 4 },
                    { 6, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_polygon_TeamId",
                table: "tbl_polygon",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_point_TeamId",
                table: "tbl_point",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_line_TeamId",
                table: "tbl_line",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_line_tbl_team_TeamId",
                table: "tbl_line",
                column: "TeamId",
                principalTable: "tbl_team",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_point_tbl_team_TeamId",
                table: "tbl_point",
                column: "TeamId",
                principalTable: "tbl_team",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_polygon_tbl_team_TeamId",
                table: "tbl_polygon",
                column: "TeamId",
                principalTable: "tbl_team",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_line_tbl_team_TeamId",
                table: "tbl_line");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_point_tbl_team_TeamId",
                table: "tbl_point");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_polygon_tbl_team_TeamId",
                table: "tbl_polygon");

            migrationBuilder.DropIndex(
                name: "IX_tbl_polygon_TeamId",
                table: "tbl_polygon");

            migrationBuilder.DropIndex(
                name: "IX_tbl_point_TeamId",
                table: "tbl_point");

            migrationBuilder.DropIndex(
                name: "IX_tbl_line_TeamId",
                table: "tbl_line");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, 4 });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "tbl_polygon");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "tbl_point");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "tbl_line");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9053), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9054) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9057), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9057) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9058), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9059) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9060), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(9060) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8935), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8936) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8939), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8939) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8940), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8940) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8941), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8942) });
        }
    }
}

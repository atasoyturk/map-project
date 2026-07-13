using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAnnotationAndRenameRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tbl_team",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_team", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_annotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoteText = table.Column<string>(type: "text", nullable: false),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_annotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_annotation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_annotation_tbl_team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "tbl_team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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
                columns: new[] { "CreatedDate", "ModifiedDate", "Name" },
                values: new object[] { new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8939), new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8939), "Çalışan" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8941), true, false, new DateTime(2026, 7, 13, 8, 34, 29, 447, DateTimeKind.Utc).AddTicks(8942), null, "Takım Lideri" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                table: "Users",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_annotation_TeamId",
                table: "tbl_annotation",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_annotation_UserId",
                table: "tbl_annotation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_tbl_team_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "tbl_team",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_tbl_team_TeamId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "tbl_annotation");

            migrationBuilder.DropTable(
                name: "tbl_team");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeamId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Users");

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
                columns: new[] { "CreatedDate", "ModifiedDate", "Name" },
                values: new object[] { new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(651), new DateTime(2026, 7, 8, 9, 13, 19, 290, DateTimeKind.Utc).AddTicks(651), "User" });
        }
    }
}

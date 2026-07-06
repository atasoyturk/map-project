using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateAndModifiedUserIdToGeoEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "tbl_polygon",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModifiedUserId",
                table: "tbl_polygon",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "tbl_point",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModifiedUserId",
                table: "tbl_point",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "tbl_line",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModifiedUserId",
                table: "tbl_line",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "tbl_polygon");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "tbl_polygon");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "tbl_point");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "tbl_point");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "tbl_line");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "tbl_line");
        }
    }
}

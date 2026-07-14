using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeadRoleColumnFromUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9395), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9395) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9396), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9396) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9398), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9398) });

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

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9230), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9230) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9495), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9495) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9496), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9496) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9497), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9498) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9499), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9499) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9499), new DateTime(2026, 7, 14, 9, 38, 10, 188, DateTimeKind.Utc).AddTicks(9500) });
        }
    }
}

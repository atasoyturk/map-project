using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPoiCategoryIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "tbl_poi_category",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "🍽️");

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: null);

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Icon", "Name", "ParentCategoryId" },
                values: new object[] { null, "Fast Food", 1 });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Icon", "Name", "ParentCategoryId" },
                values: new object[] { "🏥", "Sağlık", null });

            migrationBuilder.InsertData(
                table: "tbl_poi_category",
                columns: new[] { "Id", "CreatedDate", "Icon", "IsActive", "IsDeleted", "ModifiedDate", "ModifiedUserId", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hastane", 5 },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Eczane", 5 },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Klinik", 5 },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Diş", 5 },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🎓", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Eğitim", null },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🛒", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Market/Perakende", null },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🏦", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Finans", null },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🚉", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ulaşım", null },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🏛️", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kamu", null },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🕌", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "İbadet", null },
                    { 33, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "🏋️", true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Spor", null },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Anaokulu", 10 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Okul", 10 },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Üniversite", 10 },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kurs", 10 },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Süpermarket", 15 },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Market/Bakkal", 15 },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "AVM", 15 },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fırın", 15 },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Banka", 20 },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ATM", 20 },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Raylı İstasyon", 23 },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Otopark", 23 },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Akaryakıt", 23 },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Belediye", 27 },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Karakol", 27 },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Postane", 27 },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cami", 31 },
                    { 34, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Spor Salonu", 33 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "tbl_poi_category");

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "ParentCategoryId" },
                values: new object[] { "Turizm", null });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "ParentCategoryId" },
                values: new object[] { "Müze", 4 });
        }
    }
}

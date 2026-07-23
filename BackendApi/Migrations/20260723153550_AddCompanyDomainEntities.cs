using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_company_category",
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
                    table.PrimaryKey("PK_tbl_company_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyCategoryId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_company_tbl_company_category_CompanyCategoryId",
                        column: x => x.CompanyCategoryId,
                        principalTable: "tbl_company_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_company_route",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    TransitRouteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_company_route", x => new { x.CompanyId, x.TransitRouteId });
                    table.ForeignKey(
                        name: "FK_tbl_company_route_tbl_company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "tbl_company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_company_route_tbl_transit_route_TransitRouteId",
                        column: x => x.TransitRouteId,
                        principalTable: "tbl_transit_route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_vehicle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlateNumber = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_vehicle_tbl_company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "tbl_company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_shipment_record",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransitRouteId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_shipment_record", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_shipment_record_tbl_transit_route_TransitRouteId",
                        column: x => x.TransitRouteId,
                        principalTable: "tbl_transit_route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_shipment_record_tbl_vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "tbl_vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6262), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6262) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6267), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6267) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6269), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6269) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6271), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6271) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6273), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6273) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6274), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6274) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6276), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6276) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6277), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6277) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6279), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6279) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6280), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6281) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6282), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6282) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6283), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6283) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6285), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6285) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6286), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6287) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6288), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6288) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6076), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6078) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6082), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6082) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6084), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6084) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6085), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6086) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6087), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6089), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6089) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6090), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6090) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6505), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6506) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6508), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6508) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6510), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6511), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6512) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6513), new DateTime(2026, 7, 23, 15, 35, 49, 620, DateTimeKind.Utc).AddTicks(6513) });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_company_CompanyCategoryId",
                table: "tbl_company",
                column: "CompanyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_company_route_TransitRouteId",
                table: "tbl_company_route",
                column: "TransitRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_shipment_record_TransitRouteId",
                table: "tbl_shipment_record",
                column: "TransitRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_shipment_record_VehicleId",
                table: "tbl_shipment_record",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_vehicle_CompanyId",
                table: "tbl_vehicle",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_company_route");

            migrationBuilder.DropTable(
                name: "tbl_shipment_record");

            migrationBuilder.DropTable(
                name: "tbl_vehicle");

            migrationBuilder.DropTable(
                name: "tbl_company");

            migrationBuilder.DropTable(
                name: "tbl_company_category");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(622), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(622) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(625), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(626) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(627), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(628) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(629), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(629) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(631), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(631) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(632), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(633) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(634), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(634) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(636), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(636) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(637), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(638) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(639), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(639) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(640), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(641) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(642), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(642) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(644), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(644) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(645), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(646) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(647), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(647) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(425), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(429) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(433), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(433) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(435), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(435) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(436), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(437) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(438), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(438) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(439), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(440) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(441), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(441) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(863), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(863) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(865), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(866) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(867), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(867) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(869), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(869) });

            migrationBuilder.UpdateData(
                table: "tbl_poi_category",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(870), new DateTime(2026, 7, 23, 11, 38, 8, 115, DateTimeKind.Utc).AddTicks(870) });
        }
    }
}

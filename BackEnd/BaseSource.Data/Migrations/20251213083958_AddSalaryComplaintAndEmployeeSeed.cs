using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryComplaintAndEmployeeSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalaryComplaints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    SalarySlipId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ComplaintType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedById = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResolvedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryComplaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryComplaints_AppUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryComplaints_AppUsers_ResolvedById",
                        column: x => x.ResolvedById,
                        principalTable: "AppUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalaryComplaints_Salaries_SalarySlipId",
                        column: x => x.SalarySlipId,
                        principalTable: "Salaries",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "85506e5c-388d-4955-97d6-b51d6b51c6e1");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-employee",
                column: "ConcurrencyStamp",
                value: "de834e18-f57c-48f6-ae59-24d44b8ebb7e");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-hr",
                column: "ConcurrencyStamp",
                value: "5059bcc4-26c2-4482-8b68-61d5c1ea9fb6");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-manager",
                column: "ConcurrencyStamp",
                value: "55694146-d48d-4667-b010-0a877bc3b266");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-superadmin",
                column: "ConcurrencyStamp",
                value: "4e5d2445-e807-44e5-a65c-67efe1516fe8");

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "AnnualLeaveDays", "BaseSalary", "CreatedTime", "DateOfBirth", "DepartmentId", "Email", "EmployeeCode", "FullName", "Gender", "JoinDate", "Level", "Phone", "Position", "ProbationEndDate", "RemainingLeaveDays", "Status", "UpdatedTime", "UserId" },
                values: new object[] { "employee-superadmin", null, 12, 50000000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "admin@talentpulse.com", "ADMIN001", "Super Admin", (byte)0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), (byte)1, null, null, null, 12m, (byte)0, null, "user-superadmin" });

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5982));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5989));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5973));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5965));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(3741));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5924));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5934));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5942));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5950));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5958));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6062));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6070));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6054));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6046));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(5997));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6007));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6014));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6022));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6030));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 8, 39, 57, 268, DateTimeKind.Utc).AddTicks(6038));

            migrationBuilder.CreateIndex(
                name: "IX_SalaryComplaints_EmployeeId",
                table: "SalaryComplaints",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryComplaints_ResolvedById",
                table: "SalaryComplaints",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryComplaints_SalarySlipId",
                table: "SalaryComplaints",
                column: "SalarySlipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryComplaints");

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: "employee-superadmin");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "0c219892-68ea-4ae0-9f8b-79bd2b9fccdd");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-employee",
                column: "ConcurrencyStamp",
                value: "8f6a83b4-8972-4702-9c2f-69da370b474f");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-hr",
                column: "ConcurrencyStamp",
                value: "21fe5faf-1c62-49c2-b4ce-edfb32c05162");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-manager",
                column: "ConcurrencyStamp",
                value: "e12f93ee-ac61-4d7e-a662-ea1c1dad9170");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-superadmin",
                column: "ConcurrencyStamp",
                value: "8ccbe256-e571-4d00-bd49-be98eda8774a");

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8443));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8453));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8434));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8426));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(5548));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8375));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8386));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8396));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8406));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8418));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8531));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8593));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8523));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8515));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8463));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8473));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8482));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8491));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8499));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 3, 36, 43, 959, DateTimeKind.Utc).AddTicks(8508));
        }
    }
}

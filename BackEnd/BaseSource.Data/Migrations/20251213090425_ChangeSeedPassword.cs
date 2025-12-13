using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSeedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "e830d98e-d945-4161-aedc-9a02598e56df");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-employee",
                column: "ConcurrencyStamp",
                value: "1975863e-ee69-46d3-9d39-7d112550aa69");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-hr",
                column: "ConcurrencyStamp",
                value: "7328352a-a71d-4add-b426-f987e8e50f3b");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-manager",
                column: "ConcurrencyStamp",
                value: "7c2c5846-955f-4256-923a-3c0ef8d8cb43");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-superadmin",
                column: "ConcurrencyStamp",
                value: "2c0a77e6-676d-4842-a013-8130a44b3e02");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "user-superadmin",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEI2hj0o9j/KgKOviJLstS00vhYK3I1C0OYS15eGZqZArTRdH6WmJkkBXIydqpvFgrg==");

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4065));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4072));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4058));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4052));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(2429));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(3978));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(3985));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4038));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4045));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4134));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4127));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4120));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4079));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4087));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4094));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4100));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4107));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 13, 9, 4, 24, 707, DateTimeKind.Utc).AddTicks(4114));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: "user-superadmin",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMNmMg8QG2XUfdQgdFvVxD8dYi2JdZI+j5cK/o3TMg3Kl2nH8/pL/wW2tl+Xk6sXpA==");

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
        }
    }
}

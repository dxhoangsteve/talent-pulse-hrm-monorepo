using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseSource.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceGPSAndSalaryPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInLocation",
                table: "Attendances");

            migrationBuilder.AddColumn<int>(
                name: "EarlyLeaveDays",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LateDays",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaidBy",
                table: "Salaries",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckInAccuracy",
                table: "Attendances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLatitude",
                table: "Attendances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLongitude",
                table: "Attendances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutAccuracy",
                table: "Attendances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutLatitude",
                table: "Attendances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutLongitude",
                table: "Attendances",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMockedLocation",
                table: "Attendances",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_ApprovedBy",
                table: "Salaries",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_PaidBy",
                table: "Salaries",
                column: "PaidBy");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_PaidTime",
                table: "Salaries",
                column: "PaidTime");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRequests_ApprovedBy",
                table: "OvertimeRequests",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_ApprovedBy",
                table: "LeaveRequests",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_IsMockedLocation",
                table: "Attendances",
                column: "IsMockedLocation");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AppUsers_ApprovedBy",
                table: "LeaveRequests",
                column: "ApprovedBy",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeRequests_AppUsers_ApprovedBy",
                table: "OvertimeRequests",
                column: "ApprovedBy",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_AppUsers_ApprovedBy",
                table: "Salaries",
                column: "ApprovedBy",
                principalTable: "AppUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_AppUsers_PaidBy",
                table: "Salaries",
                column: "PaidBy",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AppUsers_ApprovedBy",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OvertimeRequests_AppUsers_ApprovedBy",
                table: "OvertimeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_AppUsers_ApprovedBy",
                table: "Salaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_AppUsers_PaidBy",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_ApprovedBy",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_PaidBy",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_PaidTime",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_OvertimeRequests_ApprovedBy",
                table: "OvertimeRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_ApprovedBy",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_IsMockedLocation",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "EarlyLeaveDays",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "LateDays",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "PaidBy",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "CheckInAccuracy",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckInLatitude",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckInLongitude",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckOutAccuracy",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckOutLatitude",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CheckOutLongitude",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "IsMockedLocation",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "CheckInLocation",
                table: "Attendances",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-admin",
                column: "ConcurrencyStamp",
                value: "b1b25cae-46ee-47e4-85ea-04b06761de93");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-employee",
                column: "ConcurrencyStamp",
                value: "f40bc42c-090b-439d-b0d2-c15ba1bb3ffd");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-hr",
                column: "ConcurrencyStamp",
                value: "7fa4d1ef-814c-4de5-9b51-1202d61c3cb1");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-manager",
                column: "ConcurrencyStamp",
                value: "ef6b0378-cd09-4dcd-903d-e423e15b97c2");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: "role-superadmin",
                column: "ConcurrencyStamp",
                value: "92b3f6fd-6848-4ab0-b290-f188e477f14c");

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6867));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6875));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6859));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6851));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(5092));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6806));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6815));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6823));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6834));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2024-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6843));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-015",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6950));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-029",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6958));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-304",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6942));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-hung",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6934));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-newyear",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6883));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet1",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6892));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet2",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6901));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet3",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6909));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet4",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6917));

            migrationBuilder.UpdateData(
                table: "Holidays",
                keyColumn: "Id",
                keyValue: "hol-2025-tet5",
                column: "CreatedTime",
                value: new DateTime(2025, 12, 12, 16, 33, 8, 719, DateTimeKind.Utc).AddTicks(6925));
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("74fd615e-c2db-4642-915b-0ddaff065487"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9c1bb2c4-8f27-468b-8621-befa76787c8a"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("445a07b5-a530-422d-9e6c-c7316f0cc1ba"), null, "Admin", "ADMIN" },
                    { new Guid("7805a370-4064-4206-ae91-4fa0e690133d"), null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("0578f87f-80a0-42ac-838b-3e4ae82a31dc"), 0, "Hey, I'm Michael.", "e23a2e1b-4ebb-428e-b7f3-f9263359bb66", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("158da52e-b441-49b6-9c2b-f49016b48f58"), 0, "Hello, I'm John.", "627f84d1-2814-45f4-8b58-afb33b1558e6", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" },
                    { new Guid("16d9c791-0848-421f-8bf0-96c16563fdcc"), 0, "Hi, I'm Jane.", "71213803-5d22-47b3-a97e-4a48c90db428", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("7805a370-4064-4206-ae91-4fa0e690133d"), new Guid("0578f87f-80a0-42ac-838b-3e4ae82a31dc") },
                    { new Guid("445a07b5-a530-422d-9e6c-c7316f0cc1ba"), new Guid("158da52e-b441-49b6-9c2b-f49016b48f58") },
                    { new Guid("7805a370-4064-4206-ae91-4fa0e690133d"), new Guid("16d9c791-0848-421f-8bf0-96c16563fdcc") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("7805a370-4064-4206-ae91-4fa0e690133d"), new Guid("0578f87f-80a0-42ac-838b-3e4ae82a31dc") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("445a07b5-a530-422d-9e6c-c7316f0cc1ba"), new Guid("158da52e-b441-49b6-9c2b-f49016b48f58") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("7805a370-4064-4206-ae91-4fa0e690133d"), new Guid("16d9c791-0848-421f-8bf0-96c16563fdcc") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("445a07b5-a530-422d-9e6c-c7316f0cc1ba"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7805a370-4064-4206-ae91-4fa0e690133d"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("0578f87f-80a0-42ac-838b-3e4ae82a31dc"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("158da52e-b441-49b6-9c2b-f49016b48f58"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("16d9c791-0848-421f-8bf0-96c16563fdcc"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("74fd615e-c2db-4642-915b-0ddaff065487"), null, "Admin", "Admin" },
                    { new Guid("9c1bb2c4-8f27-468b-8621-befa76787c8a"), null, "User", "User" }
                });
        }
    }
}

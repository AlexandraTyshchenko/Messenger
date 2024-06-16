using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_SenderId",
                table: "Messages");

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
                    { new Guid("2e41c542-256a-4ca3-8078-82462ed06e33"), null, "User", "USER" },
                    { new Guid("45335723-f8b0-4a0a-b1e7-498904fc4033"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("5830e58e-a820-49c8-b1c2-3d45a7ec60c7"), 0, "Hi, I'm Jane.", "8397d190-ed2f-4ee5-a677-2a0b370a473a", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" },
                    { new Guid("b7cffcf1-d9a4-415f-bc92-e3749e006b60"), 0, "Hey, I'm Michael.", "961f09fb-1c79-44ba-ac56-b2ba0a68321d", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("d0ad5c81-c337-4564-9c16-44ae029a2e4d"), 0, "Hello, I'm John.", "a973b181-1012-449e-a063-d5c84972714e", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("2e41c542-256a-4ca3-8078-82462ed06e33"), new Guid("5830e58e-a820-49c8-b1c2-3d45a7ec60c7") },
                    { new Guid("2e41c542-256a-4ca3-8078-82462ed06e33"), new Guid("b7cffcf1-d9a4-415f-bc92-e3749e006b60") },
                    { new Guid("45335723-f8b0-4a0a-b1e7-498904fc4033"), new Guid("d0ad5c81-c337-4564-9c16-44ae029a2e4d") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2e41c542-256a-4ca3-8078-82462ed06e33"), new Guid("5830e58e-a820-49c8-b1c2-3d45a7ec60c7") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2e41c542-256a-4ca3-8078-82462ed06e33"), new Guid("b7cffcf1-d9a4-415f-bc92-e3749e006b60") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("45335723-f8b0-4a0a-b1e7-498904fc4033"), new Guid("d0ad5c81-c337-4564-9c16-44ae029a2e4d") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2e41c542-256a-4ca3-8078-82462ed06e33"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("45335723-f8b0-4a0a-b1e7-498904fc4033"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("5830e58e-a820-49c8-b1c2-3d45a7ec60c7"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b7cffcf1-d9a4-415f-bc92-e3749e006b60"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d0ad5c81-c337-4564-9c16-44ae029a2e4d"));

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

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }
    }
}

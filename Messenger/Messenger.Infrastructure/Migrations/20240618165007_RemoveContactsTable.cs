using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContactsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserContacts");

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
                    { new Guid("308ff5c2-f9b8-4104-b1dc-86067bd39771"), null, "Admin", "ADMIN" },
                    { new Guid("e5d32289-6e71-441a-9876-7dadb7116914"), null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("76ffa92a-7bc2-4a43-80c7-165c6f355947"), 0, "Hello, I'm John.", "0b17ef8a-7e30-4dc0-b305-d2ef52773353", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" },
                    { new Guid("c9087f10-8ef4-4eb1-a9ef-0456da83440d"), 0, "Hey, I'm Michael.", "d8d28640-0604-4e13-a0ba-d0eb7d029dd9", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("cd27e46e-0dcb-44b6-a64d-61977d376c30"), 0, "Hi, I'm Jane.", "48424006-93ae-4bca-8f0f-8af2d76bfa81", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("308ff5c2-f9b8-4104-b1dc-86067bd39771"), new Guid("76ffa92a-7bc2-4a43-80c7-165c6f355947") },
                    { new Guid("e5d32289-6e71-441a-9876-7dadb7116914"), new Guid("c9087f10-8ef4-4eb1-a9ef-0456da83440d") },
                    { new Guid("e5d32289-6e71-441a-9876-7dadb7116914"), new Guid("cd27e46e-0dcb-44b6-a64d-61977d376c30") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("308ff5c2-f9b8-4104-b1dc-86067bd39771"), new Guid("76ffa92a-7bc2-4a43-80c7-165c6f355947") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("e5d32289-6e71-441a-9876-7dadb7116914"), new Guid("c9087f10-8ef4-4eb1-a9ef-0456da83440d") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("e5d32289-6e71-441a-9876-7dadb7116914"), new Guid("cd27e46e-0dcb-44b6-a64d-61977d376c30") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("308ff5c2-f9b8-4104-b1dc-86067bd39771"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e5d32289-6e71-441a-9876-7dadb7116914"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("76ffa92a-7bc2-4a43-80c7-165c6f355947"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c9087f10-8ef4-4eb1-a9ef-0456da83440d"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cd27e46e-0dcb-44b6-a64d-61977d376c30"));

            migrationBuilder.CreateTable(
                name: "UserContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContacts_AspNetUsers_ContactId",
                        column: x => x.ContactId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserContacts_AspNetUsers_ContactOwnerId",
                        column: x => x.ContactOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

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
                name: "IX_UserContacts_ContactId",
                table: "UserContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContacts_ContactOwnerId",
                table: "UserContacts",
                column: "ContactOwnerId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConversationIsCascadeOnDeleteMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsInConversation_Conversations_ConversationId",
                table: "ParticipantsInConversation");

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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0e413c4c-5d2f-45e5-9850-0711c27d8d05"), null, "Admin", "ADMIN" },
                    { new Guid("8baa01ff-6acf-4018-b31d-d93825ad7c48"), null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("2211b7e7-7c9e-4696-a973-db43720d2df5"), 0, "Hey, I'm Michael.", "97f711d9-8d58-42cd-b292-b5aa7b0c3d47", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("46b4695f-1759-4bf1-a797-17f97e45e7cc"), 0, "Hello, I'm John.", "cd17222f-c91b-4ee2-8b4b-f8f2d1eed1b7", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" },
                    { new Guid("bf7a2884-259d-4153-b1e6-b978fa995a23"), 0, "Hi, I'm Jane.", "a4aefcb6-e589-45a7-9451-b6448905f346", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("8baa01ff-6acf-4018-b31d-d93825ad7c48"), new Guid("2211b7e7-7c9e-4696-a973-db43720d2df5") },
                    { new Guid("0e413c4c-5d2f-45e5-9850-0711c27d8d05"), new Guid("46b4695f-1759-4bf1-a797-17f97e45e7cc") },
                    { new Guid("8baa01ff-6acf-4018-b31d-d93825ad7c48"), new Guid("bf7a2884-259d-4153-b1e6-b978fa995a23") }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsInConversation_Conversations_ConversationId",
                table: "ParticipantsInConversation",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsInConversation_Conversations_ConversationId",
                table: "ParticipantsInConversation");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("8baa01ff-6acf-4018-b31d-d93825ad7c48"), new Guid("2211b7e7-7c9e-4696-a973-db43720d2df5") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("0e413c4c-5d2f-45e5-9850-0711c27d8d05"), new Guid("46b4695f-1759-4bf1-a797-17f97e45e7cc") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("8baa01ff-6acf-4018-b31d-d93825ad7c48"), new Guid("bf7a2884-259d-4153-b1e6-b978fa995a23") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0e413c4c-5d2f-45e5-9850-0711c27d8d05"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8baa01ff-6acf-4018-b31d-d93825ad7c48"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("2211b7e7-7c9e-4696-a973-db43720d2df5"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("46b4695f-1759-4bf1-a797-17f97e45e7cc"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bf7a2884-259d-4153-b1e6-b978fa995a23"));

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

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsInConversation_Conversations_ConversationId",
                table: "ParticipantsInConversation",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }
    }
}

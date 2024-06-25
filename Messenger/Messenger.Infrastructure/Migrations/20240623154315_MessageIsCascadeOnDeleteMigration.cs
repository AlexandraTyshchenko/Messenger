using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessageIsCascadeOnDeleteMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

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
                    { new Guid("2ea6ccb8-a5d7-4890-a7a2-624e06709aa2"), null, "User", "USER" },
                    { new Guid("91b9b5aa-0ae8-405e-ae6e-4c2df0206df8"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("9757f8d4-8009-45fe-b153-3b51b75c964d"), 0, "Hi, I'm Jane.", "cdee69ab-3258-4b16-8d23-547433125859", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" },
                    { new Guid("ba2b49bd-daaa-4639-beb2-9e9af8001ad8"), 0, "Hello, I'm John.", "1b5439a5-81da-47e5-8026-8c97a9956562", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" },
                    { new Guid("c03e9e37-d596-40d5-97f3-49eff170f9bc"), 0, "Hey, I'm Michael.", "2ba9ff06-25ce-4256-8b13-96381605ba9a", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("2ea6ccb8-a5d7-4890-a7a2-624e06709aa2"), new Guid("9757f8d4-8009-45fe-b153-3b51b75c964d") },
                    { new Guid("91b9b5aa-0ae8-405e-ae6e-4c2df0206df8"), new Guid("ba2b49bd-daaa-4639-beb2-9e9af8001ad8") },
                    { new Guid("2ea6ccb8-a5d7-4890-a7a2-624e06709aa2"), new Guid("c03e9e37-d596-40d5-97f3-49eff170f9bc") }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2ea6ccb8-a5d7-4890-a7a2-624e06709aa2"), new Guid("9757f8d4-8009-45fe-b153-3b51b75c964d") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("91b9b5aa-0ae8-405e-ae6e-4c2df0206df8"), new Guid("ba2b49bd-daaa-4639-beb2-9e9af8001ad8") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2ea6ccb8-a5d7-4890-a7a2-624e06709aa2"), new Guid("c03e9e37-d596-40d5-97f3-49eff170f9bc") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2ea6ccb8-a5d7-4890-a7a2-624e06709aa2"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("91b9b5aa-0ae8-405e-ae6e-4c2df0206df8"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("9757f8d4-8009-45fe-b153-3b51b75c964d"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ba2b49bd-daaa-4639-beb2-9e9af8001ad8"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c03e9e37-d596-40d5-97f3-49eff170f9bc"));

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
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");
        }
    }
}

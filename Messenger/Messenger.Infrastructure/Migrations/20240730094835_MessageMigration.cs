using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MessageMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("02f01f19-cf14-47d4-bd87-fc3ac99da6d0"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a925a6fc-580a-47f1-9629-e42694f589d1"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("edf48b62-f332-45a5-8f7d-507439ec5a41"));

            migrationBuilder.AddColumn<bool>(
                name: "IsJoinMessage",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("18720494-30bd-4e9b-b6fe-9ae9ba631eed"), 0, "Hello, I'm John.", "69023d9f-3c19-4819-a521-1960cd7b13f4", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" },
                    { new Guid("2bfc7ef4-7962-40ed-8f22-173e87e92941"), 0, "Hey, I'm Michael.", "aca14c7c-b716-43f6-97f9-8b79254882bf", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("5871f66d-678f-4bdd-b548-f4d4b6b2af9a"), 0, "Hi, I'm Jane.", "b42e4036-aa91-43ec-9106-4e415cd05afe", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("18720494-30bd-4e9b-b6fe-9ae9ba631eed"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("2bfc7ef4-7962-40ed-8f22-173e87e92941"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("5871f66d-678f-4bdd-b548-f4d4b6b2af9a"));

            migrationBuilder.DropColumn(
                name: "IsJoinMessage",
                table: "Messages");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("02f01f19-cf14-47d4-bd87-fc3ac99da6d0"), 0, "Hey, I'm Michael.", "c6d91c7a-801b-447f-81a3-d8f6f9ded490", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("a925a6fc-580a-47f1-9629-e42694f589d1"), 0, "Hi, I'm Jane.", "f495867b-41e9-42cf-b6e8-97e3959ce031", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" },
                    { new Guid("edf48b62-f332-45a5-8f7d-507439ec5a41"), 0, "Hello, I'm John.", "6d93cf4b-690e-45a6-90b9-32e338cc8130", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" }
                });
        }
    }
}

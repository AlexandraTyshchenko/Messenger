using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Messenger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageTableMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("819d316f-5a0c-4318-83af-ca7ae7028887"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e8b340b1-dfbc-4398-be0f-3f4d8e861e27"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("f594659f-bd30-4653-a75f-8f06482de360"));

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Messages");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("1651c11e-7136-4143-9cab-e9543fabea0c"), 0, "Hi, I'm Jane.", "6bf3118f-8a2b-4f16-8efb-5c1b1034dec5", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" },
                    { new Guid("5b805aec-5c7d-48a5-bead-bf34bd7dd700"), 0, "Hey, I'm Michael.", "770405a9-dbd5-4650-a7f8-6b451f0be843", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("d4d427b8-8535-460d-824c-17b55c70008a"), 0, "Hello, I'm John.", "63466b8d-03a0-4f08-88cc-8c09c5629cb4", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ImageId",
                table: "Messages",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Images_ImageId",
                table: "Messages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Images_ImageId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ImageId",
                table: "Messages");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("1651c11e-7136-4143-9cab-e9543fabea0c"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("5b805aec-5c7d-48a5-bead-bf34bd7dd700"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d4d427b8-8535-460d-824c-17b55c70008a"));

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "ImgUrl", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("819d316f-5a0c-4318-83af-ca7ae7028887"), 0, "Hey, I'm Michael.", "56b16a4e-5262-4442-84a9-059b7efc0bb1", "michael.johnson@example.com", true, "Michael", "https://example.com/images/michael.jpg", true, "Johnson", false, null, "MICHAEL.JOHNSON@EXAMPLE.COM", "MICHAEL.JOHNSON", null, "5556667777", false, null, false, "michael.johnson" },
                    { new Guid("e8b340b1-dfbc-4398-be0f-3f4d8e861e27"), 0, "Hi, I'm Jane.", "ee22da71-c9b0-4d5a-809e-ac61ef713110", "jane.smith@example.com", true, "Jane", "https://example.com/images/jane.jpg", true, "Smith", false, null, "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH", null, "9876543210", false, null, false, "jane.smith" },
                    { new Guid("f594659f-bd30-4653-a75f-8f06482de360"), 0, "Hello, I'm John.", "dbec0c12-2d68-4a80-a342-d40e53e5aec0", "john.doe@example.com", true, "John", "https://example.com/images/john.jpg", true, "Doe", false, null, "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE", null, "1234567890", false, null, false, "john.doe" }
                });
        }
    }
}

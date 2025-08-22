using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisiTrack.Migrations.IdentityDb
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "100", 0, "f85e673e-491f-4c79-8af2-cbe4b32dc7a8", "admin@erp.com", true, null, null, false, null, "ADMIN@ERP.COM", "ADMIN@ERP.COM", "AQAAAAIAAYagAAAAEN1Mu0xENXGOFRj9ROHpZAIbQX8jvaOs+BY1DIBgL5N3e9AH3gWvnBcqYRkz4UIfxg==", null, false, "d6cf30c8-68a9-4116-a3e0-3972a1a4c7a3", false, "admin@erp.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "100" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "100" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "100");
        }
    }
}

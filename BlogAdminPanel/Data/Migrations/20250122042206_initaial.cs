using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAdminPanel.Data.Migrations
{
    /// <inheritdoc />
    public partial class initaial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1001",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "859c8a04-8728-4e21-83d7-80a1ac9e93d9", "AQAAAAIAAYagAAAAEIAUbspinapujvAG2JclnnH5czdL+pbYuYkn5o++kNjhOY3QWEGPZ9AoUZBLBsMsWA==", "1176c009-7b65-407d-91ca-0a973804c853" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1001",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "23cc3fcc-8090-4b88-a713-f62968df7dae", "AQAAAAIAAYagAAAAEGgKEwEAr+nGSv6cSt7AvOWL5yuwZxEQBZ2ZH/klaKdjU1YoW9r0Q46FbFPlsoZRPg==", "faa6ccc4-84ab-4dca-a0cd-dab8b7d40ef1" });
        }
    }
}

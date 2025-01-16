using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 16, 12, 12, 29, 222, DateTimeKind.Local).AddTicks(8917), "AQAAAAIAAYagAAAAEM2D1Ko4BI+eMVyNcqQF6eMwLsLwX6BkeJ9k1Pb84dXf7S9Ma2FPz94HnqLeU2hlgw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 16, 10, 36, 35, 510, DateTimeKind.Local).AddTicks(126), "AQAAAAIAAYagAAAAEAB4e8gaacES4TpYSS1lN3iaFtJb4qrjbROgSRaTesR1F7MaWbOe+nVA3ZykRM0mAQ==" });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 16, 10, 36, 35, 510, DateTimeKind.Local).AddTicks(126), "AQAAAAIAAYagAAAAEAB4e8gaacES4TpYSS1lN3iaFtJb4qrjbROgSRaTesR1F7MaWbOe+nVA3ZykRM0mAQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 13, 11, 28, 41, 665, DateTimeKind.Local).AddTicks(4005), "AQAAAAIAAYagAAAAEI1M42PEPZL4v+m2vSUR1gmMk+e0ln5uUQIBYo50qE2jAyKY1rzjiWDhS4HR4/lIIw==" });
        }
    }
}

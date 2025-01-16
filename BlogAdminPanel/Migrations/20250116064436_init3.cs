using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistories", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 16, 12, 14, 36, 15, DateTimeKind.Local).AddTicks(44), "AQAAAAIAAYagAAAAEKZXWZDS1A7BTk7QRo8DCFbpxAWudoP5amECLEnKsetfAEbzFCRYY7iMtZFhZTFiYg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginHistories");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 16, 12, 12, 29, 222, DateTimeKind.Local).AddTicks(8917), "AQAAAAIAAYagAAAAEM2D1Ko4BI+eMVyNcqQF6eMwLsLwX6BkeJ9k1Pb84dXf7S9Ma2FPz94HnqLeU2hlgw==" });
        }
    }
}

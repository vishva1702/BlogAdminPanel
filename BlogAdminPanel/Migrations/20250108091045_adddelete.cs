using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class adddelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCreateDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCreateDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserUpdateDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUpdateDto", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 8, 14, 40, 44, 373, DateTimeKind.Local).AddTicks(2739), "AQAAAAIAAYagAAAAEC+01lwJUWFpBnzxwyeplro4RrFLIHBW+v9f3LTvudP/BXvhAlklXwnu8BtzzS6THg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCreateDto");

            migrationBuilder.DropTable(
                name: "UserUpdateDto");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 7, 17, 0, 15, 375, DateTimeKind.Local).AddTicks(3521), "AQAAAAIAAYagAAAAEHk4wEZWyRjEum5mXhBAJ0aD2xfR27hYTHeMr6ZslumEYFKqlLJ4mxU4dYwxttn/XQ==" });
        }
    }
}

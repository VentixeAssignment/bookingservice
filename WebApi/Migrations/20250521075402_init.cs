using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerFirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustomerLastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustomerBirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CustomerStreetAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerPostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomerCity = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}

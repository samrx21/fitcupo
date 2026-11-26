using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitCupo.Bookings.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GymClassName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Room = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorName = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    ClientEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    BookedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ClientEmail",
                table: "Bookings",
                column: "ClientEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SessionId_Status",
                table: "Bookings",
                columns: new[] { "SessionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StartsAt",
                table: "Bookings",
                column: "StartsAt");

            migrationBuilder.CreateIndex(
                name: "UX_Bookings_Session_Client_Confirmed",
                table: "Bookings",
                columns: new[] { "SessionId", "ClientId" },
                unique: true,
                filter: "[Status] = 'Confirmed'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}

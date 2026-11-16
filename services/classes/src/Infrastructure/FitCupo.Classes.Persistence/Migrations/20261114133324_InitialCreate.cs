using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitCupo.Classes.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GymClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GymClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorName = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    Room = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSessions_GymClasses_GymClassId",
                        column: x => x.GymClassId,
                        principalTable: "GymClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "GymClasses",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "DurationMinutes", "MaxCapacity", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("019a4a00-0000-7000-8000-000000000001"), "Cycling", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ciclismo bajo techo con intervalos de alta intensidad.", 45, 20, "Spinning", "Active", null },
                    { new Guid("019a4a00-0000-7000-8000-000000000002"), "Yoga", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Posturas, respiración y flexibilidad para todos los niveles.", 60, 15, "Yoga", "Active", null },
                    { new Guid("019a4a00-0000-7000-8000-000000000003"), "Functional", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Circuitos con peso corporal, kettlebells y TRX.", 50, 12, "Entrenamiento funcional", "Active", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_GymClassId",
                table: "ClassSessions",
                column: "GymClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_InstructorId",
                table: "ClassSessions",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_StartsAt",
                table: "ClassSessions",
                column: "StartsAt");

            migrationBuilder.CreateIndex(
                name: "IX_GymClasses_Name",
                table: "GymClasses",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassSessions");

            migrationBuilder.DropTable(
                name: "GymClasses");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranPlanner.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AmountLines = table.Column<double>(type: "float", nullable: false),
                    DaysPerWeek = table.Column<int>(type: "int", nullable: false),
                    SurahCount = table.Column<int>(type: "int", nullable: false),
                    PrepDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromotionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LinkingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RulesLesson = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartSurah = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartAyah = table.Column<int>(type: "int", nullable: false),
                    DurationWeeks = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plans_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    SessionNumber = table.Column<int>(type: "int", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromPage = table.Column<int>(type: "int", nullable: false),
                    ToPage = table.Column<int>(type: "int", nullable: false),
                    FromLine = table.Column<int>(type: "int", nullable: false),
                    ToLine = table.Column<int>(type: "int", nullable: false),
                    PageRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plans_StudentId",
                table: "Plans",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PlanId",
                table: "Sessions",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}

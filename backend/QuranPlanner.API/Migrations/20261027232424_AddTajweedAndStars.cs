using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuranPlanner.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTajweedAndStars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSurahPromotion",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PromotionSurahName",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewRef",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Stars",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TajweedLesson",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TajweedLevel",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSurahPromotion",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "PromotionSurahName",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ReviewRef",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Stars",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TajweedLesson",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TajweedLevel",
                table: "Plans");
        }
    }
}

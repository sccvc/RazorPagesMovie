using Microsoft.EntityFrameworkCore.Migrations;

namespace RazorPagesMovie.Migrations
{
    public partial class PosterArt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterArt",
                table: "Movie",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ratings",
                table: "Movie",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterArt",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "Ratings",
                table: "Movie");
        }
    }
}

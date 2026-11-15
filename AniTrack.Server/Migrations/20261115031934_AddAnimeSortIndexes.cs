using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimeSortIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Anime_AverageScore",
                table: "Anime",
                column: "AverageScore");

            migrationBuilder.CreateIndex(
                name: "IX_Anime_Popularity",
                table: "Anime",
                column: "Popularity");

            migrationBuilder.CreateIndex(
                name: "IX_Anime_SeasonYear",
                table: "Anime",
                column: "SeasonYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Anime_AverageScore",
                table: "Anime");

            migrationBuilder.DropIndex(
                name: "IX_Anime_Popularity",
                table: "Anime");

            migrationBuilder.DropIndex(
                name: "IX_Anime_SeasonYear",
                table: "Anime");
        }
    }
}

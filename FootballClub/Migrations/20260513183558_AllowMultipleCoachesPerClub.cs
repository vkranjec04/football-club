using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballClub.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleCoachesPerClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coaches_ClubId",
                table: "Coaches");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_ClubId",
                table: "Coaches",
                column: "ClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coaches_ClubId",
                table: "Coaches");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_ClubId",
                table: "Coaches",
                column: "ClubId",
                unique: true);
        }
    }
}

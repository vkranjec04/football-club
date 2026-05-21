using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballClub.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffAndTrainingSessionIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TrainingSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Coaches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Coaches");
        }
    }
}

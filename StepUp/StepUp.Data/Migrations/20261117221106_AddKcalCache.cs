using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StepUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKcalCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "kcal_burnt",
                table: "day_results",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "kcal_eaten",
                table: "day_results",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kcal_burnt",
                table: "day_results");

            migrationBuilder.DropColumn(
                name: "kcal_eaten",
                table: "day_results");
        }
    }
}

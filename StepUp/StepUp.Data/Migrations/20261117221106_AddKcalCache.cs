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


            migrationBuilder.Sql(@"
                UPDATE day_results
                SET kcal_eaten = (
                        SELECT COALESCE(
                            SUM(CAST(ROUND(f.amount * f.serving_quantity * f.kcal_100g / 100.0) AS INTEGER)),
                            0)
                        FROM meals m
                        JOIN food_items f ON m.Id = f.meal_id
                        WHERE m.day_result_id = day_results.Id
                    ),
                    kcal_burnt = (
                        SELECT COALESCE(SUM(s.burnt_kcals), 0)
                        FROM sports s
                        WHERE s.day_result_id = day_results.Id
                    );
            ");
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

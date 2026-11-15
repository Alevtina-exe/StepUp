using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StepUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCaloriesType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "kcal_100g",
                table: "food_items",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "kcal_100g",
                table: "food_items",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}

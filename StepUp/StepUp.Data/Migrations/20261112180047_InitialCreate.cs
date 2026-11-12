using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StepUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    registration_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    full_name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    birth_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    gender = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    weight = table.Column<double>(type: "REAL", nullable: false),
                    height = table.Column<int>(type: "INTEGER", nullable: false),
                    calorie_plan = table.Column<int>(type: "INTEGER", nullable: false),
                    carbon_percent = table.Column<int>(type: "INTEGER", nullable: false),
                    protein_percent = table.Column<int>(type: "INTEGER", nullable: false),
                    fat_percent = table.Column<int>(type: "INTEGER", nullable: false),
                    sport_level = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.CheckConstraint("ck_users_calorie_plan", "calorie_plan >= 1300");
                    table.CheckConstraint("ck_users_carbon_pct", "carbon_percent BETWEEN 1 AND 100");
                    table.CheckConstraint("ck_users_email", "length(trim(email)) > 0");
                    table.CheckConstraint("ck_users_fat_pct", "fat_percent BETWEEN 1 AND 100");
                    table.CheckConstraint("ck_users_full_name", "length(trim(full_name)) > 0");
                    table.CheckConstraint("ck_users_gender", "gender IN ('Male', 'Female')");
                    table.CheckConstraint("ck_users_height", "height BETWEEN 100 AND 250");
                    table.CheckConstraint("ck_users_macros_sum", "protein_percent + fat_percent + carbon_percent = 100");
                    table.CheckConstraint("ck_users_password", "length(trim(password)) > 0");
                    table.CheckConstraint("ck_users_protein_pct", "protein_percent BETWEEN 1 AND 100");
                    table.CheckConstraint("ck_users_sport_level", "sport_level IN ('No', 'Low', 'Mid', 'High', 'Extreme')");
                    table.CheckConstraint("ck_users_weight", "weight BETWEEN 30 AND 300");
                });

            migrationBuilder.CreateTable(
                name: "day_results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    calorie_plan = table.Column<int>(type: "INTEGER", nullable: false),
                    weight = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_day_results", x => x.Id);
                    table.CheckConstraint("ck_day_results_calorie_plan", "calorie_plan >= 1300");
                    table.CheckConstraint("ck_day_results_weight", "weight BETWEEN 30 AND 300");
                    table.ForeignKey(
                        name: "fk_day_results_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    day_result_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    meal_type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meals", x => x.Id);
                    table.CheckConstraint("ck_meals_meal_type", "meal_type IN ('Breakfast', 'Lunch', 'Dinner', 'Snack')");
                    table.ForeignKey(
                        name: "fk_meals_day_results",
                        column: x => x.day_result_id,
                        principalTable: "day_results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    day_result_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    burnt_kcals = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sports", x => x.Id);
                    table.CheckConstraint("ck_sports_burnt_kcals", "burnt_kcals >= 0");
                    table.CheckConstraint("ck_sports_name", "length(trim(name)) > 0");
                    table.ForeignKey(
                        name: "fk_sports_day_results",
                        column: x => x.day_result_id,
                        principalTable: "day_results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "food_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    meal_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    product_id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    barcode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    amount = table.Column<double>(type: "REAL", nullable: false),
                    serving_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    serving_quantity = table.Column<double>(type: "REAL", nullable: false),
                    serving_is_liquid = table.Column<bool>(type: "INTEGER", nullable: false),
                    kcal_100g = table.Column<double>(type: "REAL", nullable: false),
                    protein_100g = table.Column<double>(type: "REAL", nullable: false),
                    fat_100g = table.Column<double>(type: "REAL", nullable: false),
                    carbon_100g = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_items", x => x.Id);
                    table.CheckConstraint("ck_food_items_amount", "amount >= 0");
                    table.CheckConstraint("ck_food_items_carbon_100g", "carbon_100g  >= 0");
                    table.CheckConstraint("ck_food_items_fat_100g", "fat_100g     >= 0");
                    table.CheckConstraint("ck_food_items_kcal_100g", "kcal_100g    >= 0");
                    table.CheckConstraint("ck_food_items_name", "length(trim(name)) > 0");
                    table.CheckConstraint("ck_food_items_product_id", "length(trim(product_id)) > 0");
                    table.CheckConstraint("ck_food_items_protein_100g", "protein_100g >= 0");
                    table.CheckConstraint("ck_food_items_serving_name", "length(trim(serving_name)) > 0");
                    table.CheckConstraint("ck_food_items_serving_quantity", "serving_quantity > 0");
                    table.ForeignKey(
                        name: "fk_food_items_meals",
                        column: x => x.meal_id,
                        principalTable: "meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ak_day_results_user_date",
                table: "day_results",
                columns: new[] { "user_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_day_results_date",
                table: "day_results",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_day_results_user_id",
                table: "day_results",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_food_items_barcode",
                table: "food_items",
                column: "barcode",
                filter: "barcode IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_food_items_meal_id",
                table: "food_items",
                column: "meal_id");

            migrationBuilder.CreateIndex(
                name: "ix_food_items_product_id",
                table: "food_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ak_meals_day_result_type",
                table: "meals",
                columns: new[] { "day_result_id", "meal_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_meals_day_result_id",
                table: "meals",
                column: "day_result_id");

            migrationBuilder.CreateIndex(
                name: "ix_sports_day_result_id",
                table: "sports",
                column: "day_result_id");

            migrationBuilder.CreateIndex(
                name: "ak_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "food_items");

            migrationBuilder.DropTable(
                name: "sports");

            migrationBuilder.DropTable(
                name: "meals");

            migrationBuilder.DropTable(
                name: "day_results");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

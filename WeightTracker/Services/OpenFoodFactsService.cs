using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using WeightTracker.Models;

namespace WeightTracker.Services;

public class OpenFoodFactsService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public async Task<FoodProduct> GetProductByBarcodeAsync(string barcode)
    {
        try
        {
            string apiUrl = $"https://ru.openfoodfacts.org/api/v0/product/{barcode}.json";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            if (root.GetProperty("status").GetInt32() == 0)
                return null;

            var product = root.GetProperty("product");
            var nutriments = product.GetProperty("nutriments");

            return new FoodProduct
            {
                Barcode = barcode,
                Name = product.TryGetProperty("product_name", out var nameElement) ? nameElement.GetString() : null,
                Brands = product.TryGetProperty("brands", out var brandsElement) ? brandsElement.GetString() : null,
                Cal100g = nutriments.TryGetProperty("energy-kcal_100g", out var calElement) ?
                    ParseNutrientValue<double>(calElement, 0) : 0,
                Protein100g = nutriments.TryGetProperty("proteins_100g", out var proteinElement) ?
                    ParseNutrientValue<double>(proteinElement, 0) : 0,
                Fat100g = nutriments.TryGetProperty("fat_100g", out var fatElement) ?
                    ParseNutrientValue<double>(fatElement, 0) : 0,
                Carbon100g = nutriments.TryGetProperty("carbohydrates_100g", out var carbElement) ?
                    ParseNutrientValue<double>(carbElement, 0) : 0,
                Sugars100g = nutriments.TryGetProperty("sugars_100g", out var sugarsElement) ?
                    ParseNutrientValue<double>(sugarsElement, 0) : 0,
                SaturatedFat100g = nutriments.TryGetProperty("saturated_fat_100g", out var satFatElement) ?
                    ParseNutrientValue<double>(satFatElement, 0) : 0,
                ImageUrl = product.TryGetProperty("image_url", out var imageElement) ?
                    imageElement.GetString() : null,
                ServingQuantity = product.TryGetProperty("serving_quantity", out var sqElement) ?
                    ParseNutrientValue<double>(sqElement, 0) : 0,
                NutriscoreGrade = product.TryGetProperty("nutriscore_grade", out var ngElement) ?
                    ngElement.GetString() : null,
                NovaGroup = product.TryGetProperty("nova_group", out var novaElement) ?
                    ParseNutrientValue<int>(novaElement, 0).ToString() : null
            };
            /*return new FoodProduct
            {
                Barcode = barcode,
                Name = "dildo",
                Brands = "zalupa co.",
                Cal100g = 666,
                Carbon100g = 100,
                Protein100g = 20,
                Fat100g = 20.6,
                NutriscoreGrade = "c",
                NovaGroup = "4",
                Sugars100g = 50,
                ImageUrl = null,
                ServingQuantity = 20
            };*/
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка: {e.Message}");
            return null;
        }
    }

    public async Task<ObservableCollection<FoodProduct>> SearchProductsByNameAsync(string name)
    {
        try
        {
            string apiUrl = $"https://ru.openfoodfacts.org/cgi/search.pl?search_terms={Uri.EscapeDataString(name)}&search_simple=1&action=process&json=1&page_size=20";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("products", out var productsElement) || productsElement.GetArrayLength() == 0)
                return new ObservableCollection<FoodProduct>();

            var results = new ObservableCollection<FoodProduct>();
            foreach (var productElement in productsElement.EnumerateArray())
            {
                try
                {
                    if (!productElement.TryGetProperty("product_name", out var nameElement) ||
                        string.IsNullOrEmpty(nameElement.GetString()) ||
                        !productElement.TryGetProperty("nutriments", out var nutrimentsElement))
                        continue;

                    if(
                        !nutrimentsElement.TryGetProperty("energy-kcal_100g", out var calElement) ||
                        !nutrimentsElement.TryGetProperty("proteins_100g", out var proteinElement) ||
                        !nutrimentsElement.TryGetProperty("fat_100g", out var fatElement) ||
                        !nutrimentsElement.TryGetProperty("carbohydrates_100g", out var carbElement))
                        continue;

                    double calories = ParseNutrientValue<double>(calElement, 0);
                    double protein = ParseNutrientValue<double>(proteinElement, 0);
                    double fat = ParseNutrientValue<double>(fatElement, 0);
                    double carbs = ParseNutrientValue<double>(carbElement, 0);

                    if (calories <= 0 || Math.Abs((protein + carbs) * 4 + fat * 9 - calories) > 10)
                        continue;

                    var product = new FoodProduct
                    {
                        Name = nameElement.GetString(),
                        Barcode = productElement.TryGetProperty("code", out var codeElement) ?
                            codeElement.GetString() : null,
                        Brands = productElement.TryGetProperty("brands", out var brandsElement) ?
                            brandsElement.GetString() : null,
                        Cal100g = calories,
                        Protein100g = protein,
                        Fat100g = fat,
                        Carbon100g = carbs,
                        Sugars100g = nutrimentsElement.TryGetProperty("sugars_100g", out var sugarsElement) ?
                            ParseNutrientValue<double>(sugarsElement, 0) : 0,
                        SaturatedFat100g = nutrimentsElement.TryGetProperty("saturated_fat_100g", out var satFatElement) ?
                            ParseNutrientValue<double>(satFatElement, 0) : 0,
                        ImageUrl = productElement.TryGetProperty("image_url", out var imageElement) ?
                            imageElement.GetString() : null,
                        ServingQuantity = productElement.TryGetProperty("serving_quantity", out var sqElement) ?
                            ParseNutrientValue<double>(sqElement, 0) : 0,
                        NutriscoreGrade = productElement.TryGetProperty("nutriscore_grade", out var ngElement) ?
                            ngElement.GetString() : null,
                        NovaGroup = productElement.TryGetProperty("nova_group", out var novaElement) ?
                            ParseNutrientValue<int>(novaElement, 0).ToString() : null
                    };

                    results.Add(product);
                    if (results.Count >= 10) break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки продукта: {ex.Message}");
                }
            }
            return results;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка запроса: {e.Message}");
            return new ObservableCollection<FoodProduct>();
        }
    }

    public async Task<ObservableCollection<FoodProduct>> ObservableDishCollection(Dictionary<string, object> dict, bool isFav)
    {
        var Dishes = new ObservableCollection<FoodProduct>();
        foreach (var dishes in dict)
        {
            double amount = isFav ? 100 : double.Parse(dishes.Value.ToString().Split('-')[0]);
            string? serving = isFav ? null : dishes.Value.ToString().Split('-')[1];
            var res = await GetProductByBarcodeAsync(dishes.Key);
            res.Amount = amount;
            res.Serving = serving;
            Dishes.Add(res);
        }
        return Dishes;
    }

    private static T ParseNutrientValue<T>(JsonElement element, T defaultValue) where T : struct
    {
        try
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                return typeof(T) switch
                {
                    Type t when t == typeof(double) => (T)(object)element.GetDouble(),
                    Type t when t == typeof(int) => (T)(object)element.GetInt32(),
                    Type t when t == typeof(decimal) => (T)(object)element.GetDecimal(),
                    Type t when t == typeof(float) => (T)(object)element.GetSingle(),
                    _ => defaultValue
                };
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                string stringValue = element.GetString();
                if (!string.IsNullOrEmpty(stringValue))
                {
                    if (typeof(T) == typeof(double) && double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                        return (T)(object)d;
                    if (typeof(T) == typeof(int) && int.TryParse(stringValue, out int i))
                        return (T)(object)i;
                }
            }
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}
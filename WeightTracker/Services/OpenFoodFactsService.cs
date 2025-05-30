using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeightTracker.Models;

namespace WeightTracker.Services;

public class OpenFoodFactsService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public async Task<FoodProduct> GetProductByBarcodeAsync(string barcode)
    {
        try
        {
            // Формируем URL запроса
            string apiUrl = $"https://ru.openfoodfacts.org/api/v0/product/{barcode}.json";

            // Отправляем GET-запрос
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            // Проверяем успешность запроса
            response.EnsureSuccessStatusCode();

            // Читаем ответ как строку
            string responseBody = await response.Content.ReadAsStringAsync();

            // Десериализуем JSON
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;

            // Проверяем статус ответа
            if (root.GetProperty("status").GetInt32() == 0)
            {
                return null; // Продукт не найден
            }

            // Извлекаем данные о продукте
            var product = root.GetProperty("product");
            var res = new FoodProduct
            {
                Barcode = barcode,
                Name = product.GetProperty("product_name").GetString(),
                Brands = product.GetProperty("brands").GetString(),
                Cal100g = product.GetProperty("nutriments").GetProperty("energy-kcal_100g").GetInt32(),
                Protein100g = product.GetProperty("nutriments").GetProperty("proteins_100g").GetDouble(),
                Fat100g = product.GetProperty("nutriments").GetProperty("fat_100g").GetDouble(),
                Carbon100g = product.GetProperty("nutriments").GetProperty("carbohydrates_100g").GetDouble(),
                ImageUrl = product.GetProperty("image_url").GetString()
            };
            return res;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Ошибка запроса: {e.Message}");
            return null;
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine($"Не найдено обязательное поле: {e.Message}");
            return null;
        }
    }


    public async Task<ObservableCollection<FoodProduct>> SearchProductsByNameAsync(string productName)
    {
        try
        {
            // Формируем URL запроса для поиска
            string apiUrl = $"https://ru.openfoodfacts.org/cgi/search.pl?search_terms={Uri.EscapeDataString(productName)}&search_simple=1&json=1&page_size=10";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            var products = jsonDoc.RootElement.GetProperty("products");

            var result = new ObservableCollection<FoodProduct>();

            foreach (var product in products.EnumerateArray())
            {
                try
                {
                    var foodProduct = new FoodProduct
                    {
                        Barcode = product.TryGetProperty("code", out var code) ? code.GetString() : null,
                        Name = product.TryGetProperty("product_name", out var name) ? name.GetString() : null,
                        Brands = product.TryGetProperty("brands", out var brands) ? brands.GetString() : null,
                        Cal100g = product.TryGetProperty("nutriments", out var nutriments) &&
                                 nutriments.TryGetProperty("energy-kcal_100g", out var kcal) ?
                                 kcal.GetInt32() : 0,
                        ImageUrl = product.TryGetProperty("image_url", out var img) ? img.GetString() : null
                    };

                    result.Add(foodProduct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка парсинга продукта: {ex.Message}");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка поиска: {ex.Message}");
            return new ObservableCollection<FoodProduct>();
        }
    }
}



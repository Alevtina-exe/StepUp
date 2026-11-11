using StepUp.Application.Interfaces;
using StepUp.Domain.Entities;
using System.Globalization;
using System.Text.Json;

namespace StepUp.Infrastructure.OpenFoodFacts;

public class OpenFoodFactsClient(HttpClient httpClient) : IFoodProductClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<FoodProduct?> GetByBarcodeAsync(string barcode,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return null;
        try
        {
            var response = await _httpClient.GetAsync($"/api/v0/product/{barcode}.json", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.GetProperty("status").GetInt32() == 0)
                return null;

            var product = root.GetProperty("product");
            var nutriments = product.GetProperty("nutriments");

            return MapProduct(root.GetProperty("product"), barcode);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }

    public async Task<List<FoodProduct>> SearchByNameAsync(
        string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        try
        {
            var url = "cgi/search.pl" +
                      $"?search_terms={Uri.EscapeDataString(query)}" +
                      "&search_simple=1&action=process&json=1&page_size=20";

            var response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("products", out var products) ||
                products.GetArrayLength() == 0)
                return [];

            var result = new List<FoodProduct>();
            foreach (var p in products.EnumerateArray())
            {
                var barcode = p.TryGetProperty("code", out var codeEl)
                    ? codeEl.GetString()
                    : null;

                var product = MapProduct(p, barcode ?? string.Empty);
                if (product is null)
                    continue;

                result.Add(product);
                if (result.Count >= 10)
                    break;
            }
            return result;
        }
        catch (HttpRequestException) { return []; }
        catch (JsonException) { return []; }
        catch (OperationCanceledException) { throw; }
    }

    private FoodProduct? MapProduct(JsonElement product, string barcode)
    {
        if (!product.TryGetProperty("_id", out var id) ||
            string.IsNullOrWhiteSpace(id.GetString()) ||
            !product.TryGetProperty("nutriments", out var n))
            return null;
        var calories = ParseDouble(n, "energy-kcal_100g");
        if (calories <= 0)
            return null;
        return new FoodProduct
        (
            id: id.GetString() ?? barcode,
            name: product.TryGetProperty("product_name", out var name) ? name.GetString() : null,
            barcode: barcode,
            nutrientsPer100g: new Nutrients(calories,
                                            ParseDouble(n, "proteins_100g"),
                                            ParseDouble(n, "fat_100g"),
                                            ParseDouble(n, "carbohydrates_100g")),
            mainServing: MapServing(product),
            brands: product.TryGetProperty("brands", out var brandsElement) ? brandsElement.GetString() : null,
            nutriscoreGrade: product.TryGetProperty("nutriscore_grade", out var nsg) ? nsg.GetString() : null,
            novaGroup: product.TryGetProperty("nova_group", out var nova) ? nova.GetString() : null,
            imageUrl: product.TryGetProperty("image_url", out var image) ?
                image.GetString() : $"{_httpClient.BaseAddress}/images/icons/dist/packaging.svg",
            sugars100g: ParseDouble(n, "sugars_100g")
        );
    }
    private static double ParseDouble(JsonElement element, string property)
    {
        if (element.ValueKind == JsonValueKind.Undefined)
            return 0;

        if (!element.TryGetProperty(property, out var value))
            return 0;

        return value.ValueKind switch
        {
            JsonValueKind.Number => value.GetDouble(),
            JsonValueKind.String when double.TryParse(
                value.GetString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var d) => d,
            _ => 0
        };
    }
    private static Serving? MapServing(JsonElement product)
    {
        var solidUnits = new Dictionary<string, double>()
        {
            ["mg"] = 0.001,
            ["g"] = 1,
            ["kg"] = 1000,
            ["oz"] = 28.3495,
            ["lb"] = 453.592,
            ["мг"] = 0.001,
            ["г"] = 1,
            ["кг"] = 1000,
        };
        var liquidUnits = new Dictionary<string, double>()
        {
            ["мл"] = 1,
            ["л"] = 1000,
            ["ml"] = 1,
            ["cl"] = 10,
            ["dl"] = 100,
            ["l"] = 1000,
            ["pt"] = 473.176,
            ["qt"] = 946.353,
            ["fl oz"] = 29.5735,
            ["gal"] = 3785.41,
        };
        var q = ParseDouble(product, "serving_quantity");
        if (product.TryGetProperty("serving_quantity_unit", out var qu) && q >= 0)
        {
            var qus = qu.GetString();
            if (qus != null)
            {
                qus = qus.ToLower();
                if (liquidUnits.TryGetValue(qus, out double amount))
                {
                    return new Serving("порция", amount * q, true);
                }
                else if (solidUnits.TryGetValue(qus, out amount))
                {
                    return new Serving("порция", amount + q);
                }
            }
        }
        q = ParseDouble(product, "product_quantity");
        if (product.TryGetProperty("product_quantity_unit", out qu) && q >= 0)
        {
            var qus = qu.GetString();
            if (qus != null)
            {
                qus = qus.ToLower();
                if (liquidUnits.TryGetValue(qus, out double amount))
                {
                    return new Serving("упаковка", amount * q, true);
                }
                else if (solidUnits.TryGetValue(qus, out amount))
                {
                    return new Serving("упаковка", amount + q);
                }
            }
        }
        return null;
    }

}
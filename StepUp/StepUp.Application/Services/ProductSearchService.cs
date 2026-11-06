using StepUp.Application.Interfaces;

namespace StepUp.Application.Services
{
    public class ProductSearchService(IFoodProductClient productClient)
    {
        private readonly IFoodProductClient _productClient = productClient;
        public Task<FoodProduct?> FindByBarcodeAsync(string barcode, CancellationToken ct = default) =>
            _productClient.GetByBarcodeAsync(barcode, ct);

        public Task<List<FoodProduct>> SearchAsync(string query, CancellationToken ct = default) =>
            _productClient.SearchByNameAsync(query, ct);
    }
}

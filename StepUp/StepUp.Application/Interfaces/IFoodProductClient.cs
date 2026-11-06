using System.Collections.ObjectModel;

namespace StepUp.Application.Interfaces
{
    public interface IFoodProductClient
    {
        Task<FoodProduct?> GetByBarcodeAsync(string barcode, CancellationToken ct = default);
        Task<List<FoodProduct>> SearchByNameAsync(string name, CancellationToken ct = default);
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WeightTracker.Models;
using WeightTracker.Services;
using WeightTracker.Views;

namespace WeightTracker.ModelViews
{
    partial class ProductBaseModelView : INotifyPropertyChanged
    {
        public ObservableCollection<FoodProduct> SearchResults { get; set; }
        private bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        public ICommand SearchCommand { get; }
        public IAsyncRelayCommand ReadBarcodeCommand { get; }
        private readonly INavigation _navigation;
        public OpenFoodFactsService service;

        private string _query;
        public string Query
        {
            get => _query;
            set { _query = value; OnPropertyChanged(); }
        }
        private FoodProduct _selectedItem;
        public FoodProduct SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        public ProductBaseModelView(INavigation navigation)
        {
            SearchResults = new ObservableCollection<FoodProduct>();
            SearchCommand = new Command(OnSearch);
            ReadBarcodeCommand = new AsyncRelayCommand(OnReadBarcode);
            service = new OpenFoodFactsService();
            _navigation = navigation;
        }
        private async void OnSearch()
        {
            IsVisible = false;
            if (!string.IsNullOrWhiteSpace(Query))
            {
                SearchResults.Clear();
                if (Query.All(char.IsDigit))
                {
                    if (SearchResults.Count == 0)
                    {
                        FoodProduct? res = await service.GetProductByBarcodeAsync(Query);
                        if (res != null)
                        {
                            SearchResults.Add(res);
                        }
                    }
                }
                else
                {
                    SearchResults.Clear();
                    SearchResults = await service.SearchProductsByNameAsync(Query);
                }
                if (SearchResults.Count == 0)
                {
                    IsVisible = true;
                }
            }
        }
        private async Task OnReadBarcode()
        {
            WeakReferenceMessenger.Default.Unregister<string>(this);

            var barcodeCompletionSource = new TaskCompletionSource<string>();

            await _navigation.PushAsync(new BarcodeScannerPage());

            WeakReferenceMessenger.Default.Register<string>(this, (recipient, barcode) =>
            {
                Application.Current.Dispatcher.Dispatch(() =>
                {
                    Query = barcode;
                    barcodeCompletionSource.TrySetResult(barcode); // Завершаем ожидание
                });
            });
            await barcodeCompletionSource.Task;
            OnSearch();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

namespace StepUp.Maui.Pages;

public partial class SportListPage : ContentPage, IQueryAttributable
{
    private readonly SportListViewModel _vm;
    private DateTime? _currentDate = null;
	public SportListPage(SportListViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
        BindingContext = _vm;
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Date", out var dateObj) && dateObj is DateTime date)
        {
            _currentDate = date;
        }

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync(_currentDate);
    }
}
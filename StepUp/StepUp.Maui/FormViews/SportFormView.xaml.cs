namespace StepUp.Maui.FormViews;

public partial class SportFormView : ContentView
{
    public event EventHandler<(string Name, int Kcal)>? SaveRequested;
    public event EventHandler? CancelRequested;
    public event EventHandler? DeleteRequested;

    public SportFormView()
    {
        InitializeComponent();
    }

    public void Configure(string title, bool showDelete, string? name = null, int? kcal = null)
    {
        TitleLabel.Text = title;
        DeleteButton.IsVisible = showDelete;
        if (name != null) NameEntry.Text = name;
        if (kcal != null) KcalEntry.Text = kcal.ToString();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            ShowError("Введите название");
            return;
        }
        if (!int.TryParse(KcalEntry.Text, out var kcal) || kcal <= 0)
        {
            ShowError("Введите корректное число калорий");
            return;
        }

        SaveRequested?.Invoke(this, (NameEntry.Text.Trim(), kcal));
    }

    private void OnDeleteClicked(object sender, EventArgs e)
        => DeleteRequested?.Invoke(this, EventArgs.Empty);

    private void OnCancelClicked(object sender, EventArgs e)
        => CancelRequested?.Invoke(this, EventArgs.Empty);

    private void OnEntryChanged(object sender, TextChangedEventArgs e)
        => ErrorLabel.IsVisible = false;

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}
using CommunityToolkit.Maui.Views;
using StepUp.Domain.Entities;

namespace StepUp.Maui.Popups;

public partial class AddSport : Popup<Sport?>
{
    readonly CancellationToken _ct;
    readonly Guid _id;
    public AddSport(Guid id, CancellationToken ct = default)
    {
        InitializeComponent();
        _ct = ct;
        _id = id;
        delButton.IsVisible = false;
        text.Text = "Добавить физ. нагрузку";
    }

    public AddSport(Guid id, string sportName, int kcalSpent, CancellationToken ct = default) :
        this(id, ct)
    {
        delButton.IsVisible = true; 
        text.Text = "Изменить физ. нагрузку";
        NameEntry.Text = sportName;
        KcalEntry.Text = kcalSpent.ToString();
    }

    private async void OkButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            ErrorLabel.IsVisible = true;
            ErrorLabel.Text = "Введите название";
            return;
        }

        if (!int.TryParse(KcalEntry.Text, out int kcal) || kcal <= 0)
        {
            ErrorLabel.IsVisible = true;
            ErrorLabel.Text = "Введите корректное число калорий";
            return;
        }

        await CloseAsync(new Sport(_id, NameEntry.Text.Trim(), kcal), _ct);
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null, _ct);
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null, _ct);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        ErrorLabel.IsVisible = false;
    }
}
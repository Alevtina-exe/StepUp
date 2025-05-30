using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Grpc.Core;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WeightTracker.Models;
using WeightTracker.ModelViews;

namespace WeightTracker.Views
{
	public partial class ChangeDataPage : ContentPage
	{
		UserModel _user = UserModel.MainUser;
		DatabaseModelView database;
		public ChangeDataPage()
		{
			InitializeComponent();
			GenderEntry.ValueText = UserModel.MainUser.Gender;
			NameEntry.ValueText = UserModel.MainUser.FullName;
			HeightEntry.ValueText = UserModel.MainUser.Height.ToString();
			WeightEntry.ValueText = UserModel.MainUser.Weight.ToString();
			KcalEntry.ValueText = UserModel.MainUser.CaloriePlan.ToString();
			ProteinEntry.ValueText = UserModel.MainUser.ProteinPercent.ToString();
			FatEntry.ValueText = UserModel.MainUser.FatPercent.ToString();
			CarbonEntry.ValueText = UserModel.MainUser.CarbonPercent.ToString();
			BirthDatePicker.Date = new DateTime(UserModel.MainUser.Year, UserModel.MainUser.Month, UserModel.MainUser.Day);
			BirthDatePicker.MaximumDate = DateTime.Now.AddDays(-1);
			BirthDatePicker.MinimumDate = new DateTime(1900, 1, 1);
			database = new DatabaseModelView(new Services.FirestoreService());
		}
        private async void GenderEntry_Tapped(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Выберите пол", "Отмена", null, "Мужской", "Женский");

            if (result != "Отмена")
            {
                GenderEntry.ValueText = result;
				_user.Gender = result;
            }

        }

		private async void ButtonCell_Tapped(object sender, EventArgs e)
		{
			string errorText = "";
			if (!Regex.IsMatch(NameEntry.ValueText.Trim(), @"^[а-яА-Яa-zA-Z]+$"))
			{
				errorText += "Имя содержит недопустимые символы.\n";
			}
			else
			{
				_user.FullName = NameEntry.ValueText;
			}
			double w = 0;
            if (!double.TryParse(WeightEntry.ValueText, out w) || w > 300 || w < 30)
			{
				errorText += "Допустимые значения веса: 30-300 кг\n";
			}
			else
			{
				_user.Weight = w;
			}
			int h = 0;
			if (!int.TryParse(HeightEntry.ValueText, out h) || h > 250 || h < 100)
			{
				errorText += "Допустимые значения роста: 100-250 см";
			}
			else
			{
				_user.Height = h;
			}
			int p = 0, c = 0, f = 0;
			if (!int.TryParse(ProteinEntry.ValueText, out p) ||
				!int.TryParse(CarbonEntry.ValueText, out c) ||
				!int.TryParse(FatEntry.ValueText, out f) ||
				f + c + p != 100 || f == 0 || p == 0 || c == 0)
			{
				ProteinEntry.ValueTextColor = Colors.Red;
				CarbonEntry.ValueTextColor = Colors.Red;
				FatEntry.ValueTextColor = Colors.Red;
			}
			else
			{
				_user.ProteinPercent = p;
				_user.CarbonPercent = c;
				_user.FatPercent = f;
			}
			if (!int.TryParse(KcalEntry.ValueText, out h) || h < 1300)
			{
				errorText += "Дневная норма калорий не должна быть меньше 1300 ккал\n";
			}
			else
			{
				_user.CaloriePlan = h;
			}
			if (errorText != "")
			{
				await DisplayAlert("Неверно заполненные поля", errorText.TrimEnd('\n'), "OK");
				return;
			}
			try
			{
				await database.DeleteUser(UserModel.MainUser.Username);
				await database.SaveUserData(UserModel.MainUser);
			}
			catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
			{
                await DisplayAlert("Что-то пошло не так", "Проверьте интернет-соединение", "OK");
				return;
            }
			UserModel.MainUser = _user;
			Navigation.RemovePage(this);
        }

        private void ProteinEntry_Tapped(object sender, EventArgs e)
        {
			ProteinEntry.ValueTextColor = Colors.Black;
			CarbonEntry.ValueTextColor = Colors.Black;
			FatEntry.ValueTextColor = Colors.Black;
        }

        private void CancelButton_Tapped(object sender, EventArgs e)
        {
			Navigation.RemovePage(this);
        }
    }
}
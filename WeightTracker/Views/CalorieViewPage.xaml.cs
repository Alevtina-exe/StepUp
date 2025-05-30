using WeightTracker.Models;

namespace WeightTracker.Views;

public partial class CalorieViewPage : ContentPage
{
	private Sport _sport;
	public CalorieViewPage(Sport sport)
	{
		_sport = sport;
		InitializeComponent();
	}
	private int findCalorieAmount()
	{
        double ko, res;
        switch (_sport)
        {
            case Sport.No:
                ko = 1.2;
                break;
            case Sport.Low:
                ko = 1.375;
                break;
            case Sport.Mid:
                ko = 1.55;
                break;
            case Sport.High:
                ko = 1.7;
                break;
            case Sport.Extreme:
                ko = 1.9;
                break;
            default:
                ko = 1;
                break;
        }
        if (UserModel.MainUser.Gender == "Мужской")
		{
			res = 66.5 + (13.75 * UserModel.MainUser.Weight) + (5.003 * UserModel.MainUser.Height);
			res -= (6.775 * UserModel.MainUser.Age);
		}
        else
        {
            res = 655.1 + (9.563 * UserModel.MainUser.Weight) + (1.85 * UserModel.MainUser.Height);
            res -= (4.676*UserModel.MainUser.Age);
        }
        res *= ko;
        return (int)res;
	}
}
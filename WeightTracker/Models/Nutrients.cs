using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WeightTracker.Models;

public class Nutrients
{
   public static float TotalProtein
    {
        get
        {
            if (DayResult.CurrentDay != null)
            {
                return Convert.ToSingle(DayResult.CurrentDay.Breakfast["Pr"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Lunch["Pr"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Dinner["Pr"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Snack["Pr"]);
            }
            else { return 0; }
        }
    }
    public static float TotalFat
    {
        get
        {
            if (DayResult.CurrentDay != null)
            {
                return Convert.ToSingle(DayResult.CurrentDay.Breakfast["Fat"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Lunch["Fat"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Dinner["Fat"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Snack["Fat"]);
            }
            else { return 0; }
        }
    }
    public static float TotalCarbon
    {
        get
        {
            if (DayResult.CurrentDay != null)
            {
                return Convert.ToSingle(DayResult.CurrentDay.Breakfast["Cb"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Lunch["Cb"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Dinner["Cb"]) +
                       Convert.ToSingle(DayResult.CurrentDay.Snack["Cb"]);
            }
            else { return 0; }
        }
    }
    public static float MaxProtein
    {
        get
        {
            if(DayResult.CurrentDay != null)
            {
                float CaloriePlan = (float)DayResult.CurrentDay.CaloriePlan;
                return MathF.Round((float)UserModel.MainUser.ProteinPercent / 100 * CaloriePlan / 4, 1);
            }
            else { return 0; }
        }
    }
    public static float MaxFat
    {
        get
        {
            if (DayResult.CurrentDay != null)
            {
                float CaloriePlan = (float)DayResult.CurrentDay.CaloriePlan;
                return MathF.Round((float)UserModel.MainUser.FatPercent / 100 * CaloriePlan / 9, 1);
            }
            else { return 0; }
        }
    }
    public static float MaxCarbon
    {
        get
        {
            if (DayResult.CurrentDay != null)
            {
                float CaloriePlan = (float)DayResult.CurrentDay.CaloriePlan;
                return MathF.Round((float)UserModel.MainUser.CarbonPercent / 100 * CaloriePlan / 4, 1);
            }
            else { return 0; }
        }
    }
    public static string ProteinLabelText
    {
        get => TotalProtein.ToString() + '/' + MaxProtein.ToString();
    }
    public static Color ProteinLabelTextColor
    {
        get => ColorChanger.GetInterpolatedColor(color(TotalProtein, MaxProtein));
    }
    public static string FatLabelText
    {
        get => TotalFat.ToString() + '/' + MaxFat.ToString();
    }
    public static Color FatLabelTextColor
    {
        get => ColorChanger.GetInterpolatedColor(color(TotalFat, MaxFat));
    }
    public static string CarbonLabelText
    {
        get => TotalCarbon.ToString() + '/' + MaxCarbon.ToString();
    }
    public static Color CarbonLabelTextColor
    {
        get => ColorChanger.GetInterpolatedColor(color(TotalCarbon, MaxCarbon));
    }
    private static float color(float total, float max)
    {
        return total / max > 1.5 ? 1 :
            total / max > 1 ? (total / max - 1) * 2 :
            1 - total / max;
    }

}

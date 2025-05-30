using Android.Content;
using Android.Preferences;

namespace WeightTracker.Models;
public class DailyStepCounter
{
    private const string StepsKey = "daily_steps";
    private const string DateKey = "last_step_date";

    private readonly ISharedPreferences _prefs;

    public DailyStepCounter(Context context)
    {
        _prefs = PreferenceManager.GetDefaultSharedPreferences(context);
    }

    public void SaveSteps(int totalSteps, int todaySteps)
    {
        var editor = _prefs.Edit();
        editor.PutInt(StepsKey, totalSteps);
        editor.PutInt(DateKey, DateTime.Now.DayOfYear);
        editor.PutInt("today_steps", todaySteps);
        editor.Apply();
    }

    public (int todaySteps, bool isNewDay) GetDailySteps(int currentSensorSteps)
    {
        var lastDate = _prefs.GetInt(DateKey, -1);
        var lastSteps = _prefs.GetInt(StepsKey, 0);
        var savedTodaySteps = _prefs.GetInt("today_steps", 0);

        var isNewDay = DateTime.Now.DayOfYear != lastDate;

        if (isNewDay)
        {
            return (0, true); // Новый день - обнуляем счетчик
        }

        var stepsSinceLastSave = currentSensorSteps - lastSteps;
        return (savedTodaySteps + stepsSinceLastSave, false);
    }
}
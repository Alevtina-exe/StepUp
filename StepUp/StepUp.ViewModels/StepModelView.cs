using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeightTracker.ModelViews;

public class StepModelView : INotifyPropertyChanged
{
    private string _stepsText = "0 шагов";

    public string StepsText
    {
        get => _stepsText;
        private set
        {
            if (_stepsText == value) return;
            _stepsText = value;
            OnPropertyChanged();
        }
    }

    public string Calories
    {
        get => (int.Parse(_stepsText.Split(' ')[0]) * 0.05).ToString() + " ккал";
    }

    public StepModelView()
    {
        StepCounterService.StepsUpdated += steps =>
        {
            StepsText = FormatSteps(steps);
        };
    }


    private string FormatSteps(int count)
    {
        if (count == 0) return "0 шагов";

        string stepWord = "шагов";
        if (count % 10 == 1 && count % 100 != 11)
            stepWord = "шаг";
        else if (count % 10 >= 2 && count % 10 <= 4 &&
                !(count % 100 >= 12 && count % 100 <= 14))
            stepWord = "шага";

        return $"{count} {stepWord}";
    }
    public void RefreshAll()
    {
        OnPropertyChanged(nameof(StepsText));
        OnPropertyChanged(nameof(Calories));
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
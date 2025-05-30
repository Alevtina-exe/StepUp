using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microcharts;
using Microcharts.Maui;
using SkiaSharp;
using System.Windows.Input;
using WeightTracker.Models;

namespace WeightTracker;

public partial class GraphModelView : ObservableObject
{
    private Dictionary<string, float> weekMonthDict;
    private Dictionary<string, float> yearDict;
    private DateTime _firstDay;
    private DateTime _lastDay;
    private DateTime _regDay = DTWork.FromId(UserModel.MainUser.RegistrationDate);
    public ICommand RightButtonCommand { get; }
    public ICommand LeftButtonCommand { get; }
    public GraphModelView(Dictionary<string, float> weekMonthDict, Dictionary<string, float> yearDict)
    {
        this.weekMonthDict = weekMonthDict;
        this.yearDict = yearDict;
        RightButtonCommand = new Command(OnRight);
        LeftButtonCommand = new Command(OnLeft);
    }
    public void CreateGraph (DateTime lastDay)
    {
        _lastDay = lastDay;
        double y_value = UserModel.MainUser.Weight - 1;
        List<ChartEntry> entries = new List<ChartEntry>();
        DateTime date = lastDay;
        if (SelectedInterval == "Год")
        {
            do
            {
                string str_date = date.ToString("MMM, yyyy");
                y_value = Math.Min(y_value, yearDict[str_date] - 1);
                entries.Add(new ChartEntry(yearDict[str_date])
                {
                    Label = str_date,
                    ValueLabel = yearDict[str_date].ToString(),
                    Color = SKColor.Parse("#3BBD1E")
                });
                date = date.AddMonths(-1);
            } while (date.Year != lastDay.Year - 1 &&
            date.Date >= _regDay.Date);
            _firstDay = DTWork.Max(new DateTime(lastDay.Year, 1, 1), _regDay); 
            Range = lastDay.Year.ToString() + " год";
        }
        else if(SelectedInterval == "Месяц")
        {
            do
            {
                string str_date = DTWork.DateId(date);
                y_value = Math.Min(y_value, weekMonthDict[str_date] - 1);
                entries.Add(new ChartEntry(weekMonthDict[str_date])
                {
                    Label = date.Day.ToString(),
                    ValueLabel = weekMonthDict[str_date].ToString(),
                    Color = SKColor.Parse("#3BBD1E")
                });
                date = date.AddDays(-1);
            } while (date.Month != lastDay.Month - 1 &&
            date.Date >= _regDay.Date);
            _firstDay = DTWork.Max(new DateTime(lastDay.Year, lastDay.Month, 1), _regDay);
            Range = lastDay.ToString("MMMM");
        }
        else if (SelectedInterval == "Неделя")
        {
            do
            {
                string str_date = DTWork.DateId(date);
                y_value = Math.Min(y_value, weekMonthDict[str_date] - 1);
                entries.Add(new ChartEntry(weekMonthDict[str_date])
                {
                    Label = date.ToString("ddd, dd MMM"),
                    ValueLabel = weekMonthDict[str_date].ToString(),
                    Color = SKColor.Parse("#3BBD1E")
                });
                date = date.AddDays(-1);
            } while (date.DayOfWeek != DayOfWeek.Sunday &&
            date.Date >= _regDay.Date);
            date = date.AddDays(1);
            _firstDay = date;
            Range = date.ToString("dd MMM") + " - " + lastDay.ToString("dd MMM");
        }
        entries.Reverse();
        LeftEnabled = true;
        RightEnabled = true;
        if(_firstDay.Date == _regDay.Date) LeftEnabled = false;
        if(_lastDay.Date == DateTime.Now.Date) RightEnabled = false;
        ChartData = new LineChart
        {
            MinValue = (int)y_value,
            ValueLabelOption = ValueLabelOption.TopOfElement,
            LabelColor = SKColors.Black,
            ValueLabelTextSize = 30,
            LabelTextSize = 20,
            ValueLabelOrientation = Orientation.Horizontal,
            Entries = entries
        };
    }

    [ObservableProperty]
    private string range;
    [ObservableProperty]
    LineChart chartData;
    [ObservableProperty]
    bool rightEnabled = false;
    [ObservableProperty]
    bool leftEnabled = true;
    [ObservableProperty]
    public string selectedInterval = "Неделя";

    private float _zoomLevel = 1.0f;
    public float ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            _zoomLevel = value;
            OnPropertyChanged();
            UpdateChartScale();
        }
    }
    public void OnRight()
    {
        DateTime date;
        switch(SelectedInterval)
        {
            case "Неделя":
                date = _lastDay.AddDays(7);
                break;
            case "Месяц":
                date = _lastDay.AddDays(1).AddMonths(1).AddDays(-1);
                break;
            case "Год":
                date = new DateTime(_lastDay.Year, 12, 31);
                break;
            default:
                date = DateTime.Now;
                break;
        }
        CreateGraph(DTWork.Min(date, DateTime.Now));

    }
    public void OnLeft()
    {
        CreateGraph(_firstDay.AddDays(-1));
    }
    private void UpdateChartScale()
    {
        if (ChartData is LineChart lineChart)
        {
            lineChart.LabelTextSize = 12 * ZoomLevel;
            lineChart.ValueLabelTextSize = 12 * ZoomLevel;
            lineChart.LineSize = 3 * ZoomLevel;
        }
    }
}

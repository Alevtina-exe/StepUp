using StepUp.Application.Interfaces.Services;
using StepUp.Presentation.Models;
using System.Collections.ObjectModel;

namespace StepUp.Presentation.ViewModels
{
    public partial class GraphViewModel(ICurrentUserService user,
        DayResultService dayService, IClock clock) : ObservableObject
    {
        private DateTime _registrationDate;
        [ObservableProperty]
        private ObservableCollection<DoubleChartPoint> _weightPoints = [];
        [ObservableProperty]
        private ObservableCollection<IntChartPoint> _kcalEatenPoints = [];
        [ObservableProperty]
        private ObservableCollection<IntChartPoint> _kcalBurntPoints = [];

        public List<TimeRange> Ranges { get; } = TimeRange.All;
        [ObservableProperty]
        public TimeRange _weightRange = TimeRange.Week;
        [ObservableProperty]
        public TimeRange _kcalRange = TimeRange.Week;

        private DateTime _weightTo = clock.Today;
        private DateTime _kcalTo = clock.Today;

        [ObservableProperty]
        private bool _weightNextEnabled = false;
        [ObservableProperty]
        private bool _weightPrevEnabled = true;
        [ObservableProperty]
        private bool _kcalNextEnabled = false;
        [ObservableProperty]
        private bool _kcalPrevEnabled = true;


        private List<DoubleChartPoint> _weightPointsCache = [];
        private List<IntChartPoint> _kcalEatenPointsCache = []; 
        private List<IntChartPoint> _kcalBurntPointsCache = []; 

        public async Task InitializeAsync()
        {
            var today = clock.Today;
            _registrationDate = 
                (await user.GetOrLoadAsync()).RegistrationDate.Date;
            var dtos = await dayService.GetRangeForChart(_registrationDate, today);
            _weightPointsCache = dtos
                .Select(p => new DoubleChartPoint(p.Date, p.Weight))
                .ToList();
            _kcalEatenPointsCache = dtos 
                .Select(p => new IntChartPoint(p.Date, p.KcalEaten)) 
                .ToList();
            _kcalEatenPointsCache = dtos
                .Select(p => new IntChartPoint(p.Date, p.KcalBurnt))
                .ToList();
            ChangeWeightPoints(WeightRange.DurationInDays);
            ChangeKcalPoints(KcalRange.DurationInDays);
        }

        private void ChangeWeightPoints(int range)
        {
            WeightPrevEnabled = true;
            var to = _weightTo;
            var from = to.AddDays(-range);
            WeightPoints = new ObservableCollection<DoubleChartPoint>
                (_weightPointsCache.Where(p => p.Date <= to && p.Date >= from));
            if(WeightPoints.Count < range)
            {
                WeightPrevEnabled = false;
                for(int i = 0; WeightPoints.Count < range; from.AddDays(1), i++)
                {
                    WeightPoints.Insert(i, new DoubleChartPoint(from, null));
                }
            }
        }
        private void ChangeKcalPoints(int range)
        {
            KcalPrevEnabled = true;
            var to = _kcalTo;
            var from = to.AddDays(-range);
            KcalEatenPoints = new ObservableCollection<IntChartPoint>
                (_kcalEatenPointsCache.Where(p => p.Date <= to &&  to.Date >= from));
            KcalBurntPoints = new ObservableCollection<IntChartPoint>
                (_kcalBurntPointsCache.Where(p => p.Date <= to && to.Date >= from));
            if (KcalEatenPoints.Count < range)
            {
                KcalPrevEnabled = false;
                for (int i = 0; KcalEatenPoints.Count < range; from.AddDays(1), i++)
                {
                    KcalEatenPoints.Insert(i, new IntChartPoint(from, null));
                    KcalBurntPoints.Insert(i, new IntChartPoint(from, null));
                }
            }
        }
        partial void OnWeightRangeChanged(TimeRange value)
        {
            ChangeWeightPoints(value.DurationInDays);
        }
        partial void OnKcalRangeChanged(TimeRange value)
        {
            ChangeKcalPoints(value.DurationInDays);
        }
        [RelayCommand]
        private void OnWeightNext()
        {
            if(_weightTo.AddDays(WeightRange.DurationInDays) >= clock.Today)
            {
                _weightTo = clock.Today;
                WeightNextEnabled = false;
            }
            else _weightTo = _weightTo.AddDays(WeightRange.DurationInDays);
            ChangeWeightPoints(WeightRange.DurationInDays);
        }
        [RelayCommand]
        private void OnKcalNext()
        {
            if(_kcalTo.AddDays(KcalRange.DurationInDays) >= clock.Today)
            {
                _kcalTo = clock.Today;
                KcalNextEnabled = false;
            }
            else _kcalTo = _kcalTo.AddDays(KcalRange.DurationInDays);
            ChangeKcalPoints(KcalRange.DurationInDays);
        }
        [RelayCommand]
        private void OnWeightPrev()
        {
            _weightTo = _weightTo.AddDays(-WeightRange.DurationInDays);
            ChangeWeightPoints(WeightRange.DurationInDays);
            if(_weightTo.AddDays(-WeightRange.DurationInDays -1) <= _registrationDate)
                WeightPrevEnabled = false;
            else _weightTo = _weightTo.AddDays(WeightRange.DurationInDays);
        }
        [RelayCommand]
        private void OnKcalPrev()
        {
            _kcalTo = _kcalTo.AddDays(-KcalRange.DurationInDays);
            ChangeKcalPoints(KcalRange.DurationInDays);
            if(_kcalTo.AddDays(-KcalRange.DurationInDays -1) <= _registrationDate)
                KcalPrevEnabled = false;
            else _kcalTo = _kcalTo.AddDays(KcalRange.DurationInDays);
        }
    }
}

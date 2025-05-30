using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Models;
using static Java.Util.Jar.Attributes;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace WeightTracker.ModelViews
{
    public class ProductPageModelView : INotifyPropertyChanged
    {
        private FoodProduct _product;
        private double _selectedItem;
        public string SelectedItem
        {
            get => DoublePortionToString(_selectedItem);
            set
            {
                _selectedItem = StringPortionToDouble(value);
                CurrentPortion_gr =  _selectedItem * _amount;
                OnPropertyChanged();
            }
        }
        private double _amount;
        public string Amount
        {
            get => Math.Round(_amount, 2).ToString();
            set
            {
                double w;
                if(Double.TryParse(value, out w))
                {
                    _amount = w;
                    CurrentPortion_gr = _selectedItem * _amount;
                }
            }
        }

        private double _currentPortion_gr;
        public double CurrentPortion_gr
        {
            get => _currentPortion_gr;
            set
            {
                _currentPortion_gr = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Protein));
                OnPropertyChanged(nameof(Fat));
                OnPropertyChanged(nameof(Carbon));
                OnPropertyChanged(nameof(Kcal));
            }
        }
        private Meal _mealItem;
        public string MealItem
        {
            get => _mealItem.GetDescription();
            set
            {
                switch(value)
                {
                    case "Завтрак":
                        _mealItem = Meal.Breakfast;
                        break;
                    case "Обед":
                        _mealItem = Meal.Lunch;
                        break;
                    case "Ужин":
                        _mealItem = Meal.Dinner;
                        break;
                    case "Перекус":
                        _mealItem = Meal.Snack;
                        break;
                    default:
                        break;
                }
                OnPropertyChanged();
            }

        }
        private double StringPortionToDouble (string portion)
        {
            switch (portion)
            {
                case "100 г":
                    return 100.0;
                case "г":
                    return 1.0;
                case "порция":
                    return 100.0;
                default:
                    return 0;
            }
        }
        private string DoublePortionToString(double portion)
        {
            switch(portion)
            {
                case 100:
                    return "100 г";
                case 1:
                    return "г";
                default:
                    return "порция";
            }
        }
        public FoodProduct Product
        {
            get => _product;
            set
            {
                _product = value;
            }
        }
        public ImageSource Image
        {
            get
            {
                if (Product.ImageUrl != null)
                    return ImageSource.FromUri(new Uri(Product.ImageUrl));
                else
                    return ImageSource.FromFile("Resources/Images/no_image.jpeg");
            }
        }
        public string Name => _product?.Name ?? "Нет данных";
        public string Kcal => Math.Round(_currentPortion_gr / 100.0 * _product.Cal100g, 1).ToString() + " ккал\n(" +
            Math.Round(_currentPortion_gr / 100.0 * _product.Cal100g / DayResult.CurrentDay.CaloriePlan * 100.0).ToString() +
            "% от дневной нормы)" ?? "Нет данных";
        public string Protein => "Белки: " + Math.Round(_currentPortion_gr / 100.0 * _product.Protein100g, 1).ToString() + " г" ?? string.Empty;
        public string Fat => "Жиры: " + Math.Round(_currentPortion_gr / 100.0 * _product.Fat100g, 1).ToString() + " г" ?? string.Empty;
        public string Carbon => "Углеводы: " + Math.Round(_currentPortion_gr / 100.0 * _product.Carbon100g, 1).ToString() + " г" ?? string.Empty;
        public string NameForSearch => _product?.NameForSearch ?? "Нет данных";
        public string ImageUrl => _product?.ImageUrl ?? string.Empty;
        private double _proteinPercentage;
        public double ProteinPercentage 
        {
            get => Math.Round(_proteinPercentage * 100, 1);
            set
            {
                _proteinPercentage = value;
                OnPropertyChanged(nameof(ProteinPercentage));
            }
         }
        private double _fatPercentage;
        public double FatPercentage
        {
            get => Math.Round(_fatPercentage * 100, 1);
            set
            {
                _fatPercentage = value;
                OnPropertyChanged(nameof(FatPercentage));
            }
        }
        public double CarbonPercentage => Math.Round(100.0 - ProteinPercentage - FatPercentage, 1);
        public bool ProteinILA => _proteinPercentage > 0.5;
        public Point FatStartPoint => GetPointOnCircle(_proteinPercentage);
        public bool FatILA => _fatPercentage > 0.5;
        public Point CarbonStartPoint => GetPointOnCircle(_fatPercentage, _proteinPercentage);
        public bool CarbonILA => CarbonPercentage > 50;

        private static Point GetPointOnCircle(double progress, double offset = 0)
        {
            double radius = 50, centerX = 70, centerY = 70;
            double angleRadians = -Math.PI * 2 * (progress + offset);
            double x = (centerX + radius * Math.Cos(angleRadians));
            double y = (centerY + radius * Math.Sin(angleRadians));
            return new Point(x, y);
        }
        private Path? _prpath;
        private Path? _fatpath;
        private Path? _cbpath;
        private void CreateDiagramm(Grid grid)
        {
            _prpath = new Path
            {
                Stroke = Color.FromArgb("#9fd5bd"),
                StrokeThickness = 20,
                Fill = Colors.Transparent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = GetPointOnCircle(0),
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = FatStartPoint,
                                    Size = new Size(50, 50),
                                    IsLargeArc = ProteinILA,
                                    SweepDirection = SweepDirection.CounterClockwise
                                }
                            }
                        }
                    }
                }
            };
            grid.Children.Add(_prpath);
            Grid.SetColumn(_prpath, 0);
            _fatpath = new Path
            {
                Stroke = Color.FromArgb("#fceda8"),
                StrokeThickness = 20,
                Fill = Colors.Transparent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection {
                        new PathFigure
                        {
                            StartPoint = FatStartPoint,
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = CarbonStartPoint,
                                    Size = new Size(50,50),
                                    IsLargeArc = FatILA,
                                    SweepDirection = SweepDirection.CounterClockwise
                                }
                            }
                        }
                    }
                }
            };
            grid.Children.Add(_fatpath);
            Grid.SetColumn (_fatpath, 0);
            _cbpath = new Path
            {
                Stroke = Color.FromArgb("#e79bc3"),
                StrokeThickness = 20,
                Fill = Colors.Transparent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection {
                        new PathFigure
                        {
                            StartPoint = CarbonStartPoint,
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = GetPointOnCircle(0),
                                    Size = new Size(50,50),
                                    IsLargeArc = CarbonILA,
                                    SweepDirection = SweepDirection.CounterClockwise
                                }
                            }
                        }
                    }
                }
            };
            grid.Children.Add(_cbpath);
            Grid.SetColumn(_cbpath, 0);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ProductPageModelView(FoodProduct product, Grid grid, Meal meal)
        {
            _product = product;
            ProteinPercentage = _product.Protein100g * 4.0 / _product.Cal100g;
            FatPercentage = _product.Fat100g * 9.0 / _product.Cal100g;
            CreateDiagramm(grid);
            SelectedItem = "100 г";
            _amount = 1;
            _mealItem = meal;
        }
    }
}

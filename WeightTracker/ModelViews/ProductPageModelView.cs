using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using WeightTracker.Models;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace WeightTracker.ModelViews
{
    public class ProductPageModelView : INotifyPropertyChanged
    {
        private readonly FoodProduct _product;
        private double _selectedItem;
        private double _amount;
        private double _currentPortion_gr;
        private Meal _mealItem;
        private double _proteinPercentage;
        private double _fatPercentage;
        private Path _prpath;
        private Path _fatpath;
        private Path _cbpath;

        public ProductPageModelView(FoodProduct product, Grid grid, Meal meal)
        {
            try
            {
                _product = product ?? throw new ArgumentNullException(nameof(product));

                // Проверка обязательных значений
                if (_product.Cal100g <= 0)
                    throw new ArgumentException("Калорийность продукта должна быть больше 0");

                SelectedItem = "100 г";
                _amount = 1;
                _mealItem = meal;

                // Расчет процентов
                ProteinPercentage = _product.Protein100g * 4.0 / _product.Cal100g;
                FatPercentage = _product.Fat100g * 9.0 / _product.Cal100g;

                // Добавление варианта "порция" если есть ServingQuantity
                if (_product.ServingQuantity > 0 && _product.ServingQuantity != 100)
                {
                    MeasurementUnits.Add($"порция ({_product.ServingQuantity} г)");
                }

                CreateDiagramm(grid);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка инициализации ProductPageModelView: {ex}");
                throw;
            }
        }

        public string SelectedItem
        {
            get => DoublePortionToString(_selectedItem);
            set
            {
                try
                {
                    _selectedItem = StringPortionToDouble(value.Length > 5 ? value.Split()[0] : value);
                    CurrentPortion_gr = _selectedItem * _amount;
                    OnPropertyChanged();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка в SelectedItem: {ex}");
                    _selectedItem = 100; // Значение по умолчанию
                }
            }
        }

        public string Amount
        {
            get => Math.Round(_amount, 2).ToString();
            set
            {
                if (double.TryParse(value, out var w) && w > 0)
                {
                    _amount = w;
                    CurrentPortion_gr = _selectedItem * _amount;
                }
            }
        }

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
                OnPropertyChanged(nameof(Daily));
                OnPropertyChanged(nameof(Progress));
            }
        }

        public ObservableCollection<string> MeasurementUnits { get; } = new ObservableCollection<string> { "100 г", "г" };

        public string MealItem
        {
            get => _mealItem.GetDescription();
            set
            {
                try
                {
                    _mealItem = value switch
                    {
                        "Завтрак" => Meal.Breakfast,
                        "Обед" => Meal.Lunch,
                        "Ужин" => Meal.Dinner,
                        "Перекус" => Meal.Snack,
                        _ => _mealItem
                    };
                    OnPropertyChanged();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка установки MealItem: {ex}");
                }
            }
        }

        public ImageSource Image =>
            Uri.TryCreate(_product?.ImageUrl, UriKind.Absolute, out var uri)
                ? ImageSource.FromUri(uri)
                : ImageSource.FromFile("Resources/Images/no_image.jpeg");

        public ImageSource NutriscoreImage
        {
            get
            {
                try
                {
                    string score = "UK";
                    if (!string.IsNullOrEmpty(_product?.NutriscoreGrade) &&
                        _product.NutriscoreGrade != "unknown")
                    {
                        score = _product.NutriscoreGrade.ToUpper();
                    }

                    if (Application.Current?.Resources?.TryGetValue($"{score}Nutriscore", out var icon) ?? false)
                    {
                        return icon as ImageSource ?? ImageSource.FromFile("fallback_nutriscore.png");
                    }
                    return ImageSource.FromFile("fallback_nutriscore.png");
                }
                catch
                {
                    return ImageSource.FromFile("fallback_nutriscore.png");
                }
            }
        }

        public ImageSource NovaImage
        {
            get
            {
                try
                {
                    string score = "1"; // Значение по умолчанию
                    if (!string.IsNullOrEmpty(_product?.NovaGroup))
                    {
                        score = _product.NovaGroup;
                    }

                    if (Application.Current?.Resources?.TryGetValue($"Nova{score}", out var icon) ?? false)
                    {
                        return icon as ImageSource ?? ImageSource.FromFile("fallback_nova.png");
                    }
                    return ImageSource.FromFile("fallback_nova.png");
                }
                catch
                {
                    return ImageSource.FromFile("fallback_nova.png");
                }
            }
        }

        public string Name => _product?.Name ?? "Нет данных";
        public string Brand => $"Производитель:\n\t{_product?.Brands ?? "не указан"}";
        public string Barcode => $"Штрихкод:\n\t{_product?.Barcode ?? "не указан"}";

        public string Kcal => $"{Math.Round(_currentPortion_gr / 100.0 * _product.Cal100g, 1)} ккал";
        public string Daily => $"({(Math.Round(_currentPortion_gr / 100.0 * _product.Cal100g / (DayResult.CurrentDay?.CaloriePlan ?? 2000) * 100.0, 1))}% от дневной нормы)";
        public double Progress => _currentPortion_gr / 100.0 * _product.Cal100g / (DayResult.CurrentDay?.CaloriePlan ?? 2000);
        public string Protein => $"Белки: {Math.Round(_currentPortion_gr / 100.0 * _product.Protein100g, 1)} г";
        public string Fat => $"Жиры: {Math.Round(_currentPortion_gr / 100.0 * _product.Fat100g, 1)} г (в том числе насыщенные: {Math.Round(_currentPortion_gr / 100.0 * _product.SaturatedFat100g, 1)} г)";
        public string Carbon => $"Углеводы: {Math.Round(_currentPortion_gr / 100.0 * _product.Carbon100g, 1)} г (в том числе сахара: {Math.Round(_currentPortion_gr / 100.0 * _product.Sugars100g, 1)} г)";

        public double ProteinPercentage
        {
            get => Math.Round(_proteinPercentage * 100, 1);
            set
            {
                _proteinPercentage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CarbonPercentage));
            }
        }

        public double FatPercentage
        {
            get => Math.Round(_fatPercentage * 100, 1);
            set
            {
                _fatPercentage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CarbonPercentage));
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
            const double radius = 50, centerX = 70, centerY = 70;
            double angleRadians = -Math.PI * 2 * (progress + offset);
            return new Point(
                centerX + radius * Math.Cos(angleRadians),
                centerY + radius * Math.Sin(angleRadians)
            );
        }

        private void CreateDiagramm(Grid grid)
        {
            try
            {
                if (grid == null) return;

                // Удаляем старые элементы
                SafeRemoveFromGrid(grid, _prpath);
                SafeRemoveFromGrid(grid, _fatpath);
                SafeRemoveFromGrid(grid, _cbpath);

                // Проверка на 100% для каждого компонента
                if (ProteinPercentage >= 100)
                {
                    _prpath = CreateFullCircle(Color.FromArgb("#9fd5bd"));
                    grid.Children.Add(_prpath);
                    Grid.SetColumn(_prpath, 0);
                    return;
                }

                if (FatPercentage >= 100)
                {
                    _fatpath = CreateFullCircle(Color.FromArgb("#fceda8"));
                    grid.Children.Add(_fatpath);
                    Grid.SetColumn(_fatpath, 0);
                    return;
                }

                if (CarbonPercentage >= 100)
                {
                    _cbpath = CreateFullCircle(Color.FromArgb("#e79bc3"));
                    grid.Children.Add(_cbpath);
                    Grid.SetColumn(_cbpath, 0);
                    return;
                }

                // Создаем диаграмму с тремя сегментами
                _prpath = CreatePath(Color.FromArgb("#9fd5bd"), GetPointOnCircle(0), FatStartPoint, ProteinILA);
                _fatpath = CreatePath(Color.FromArgb("#fceda8"), FatStartPoint, CarbonStartPoint, FatILA);
                _cbpath = CreatePath(Color.FromArgb("#e79bc3"), CarbonStartPoint, GetPointOnCircle(0), CarbonILA);

                grid.Children.Add(_prpath);
                grid.Children.Add(_fatpath);
                grid.Children.Add(_cbpath);

                Grid.SetColumn(_prpath, 0);
                Grid.SetColumn(_fatpath, 0);
                Grid.SetColumn(_cbpath, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания диаграммы: {ex}");
            }
        }

        private Path CreatePath(Color color, Point start, Point end, bool isLargeArc)
        {
            return new Path
            {
                Stroke = color,
                StrokeThickness = 20,
                Fill = Colors.Transparent,
                Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = start,
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = end,
                                    Size = new Size(50, 50),
                                    IsLargeArc = isLargeArc,
                                    SweepDirection = SweepDirection.CounterClockwise
                                }
                            }
                        }
                    }
                }
            };
        }

        private Path CreateFullCircle(Color color)
        {
            return new Path
            {
                Stroke = color,
                StrokeThickness = 20,
                Fill = Colors.Transparent,
                Data = new EllipseGeometry
                {
                    Center = new Point(70, 70),
                    RadiusX = 50,
                    RadiusY = 50
                }
            };
        }

        private void SafeRemoveFromGrid(Grid grid, Path path)
        {
            if (path != null && grid.Children.Contains(path))
            {
                grid.Children.Remove(path);
            }
        }

        private double StringPortionToDouble(string portion)
        {
            if (string.IsNullOrEmpty(portion)) return 100.0;

            try
            {
                return portion switch
                {
                    "100 г" => 100.0,
                    "г" => 1.0,
                    "порция" => _product?.ServingQuantity ?? 100.0,
                    _ when portion.Contains("порция") && portion.Contains("(") =>
                        double.TryParse(Regex.Match(portion, @"\d+").Value, out var weight) ? weight : 100.0,
                    _ => 100.0
                };
            }
            catch
            {
                return 100.0;
            }
        }

        private string DoublePortionToString(double portion)
        {
            if (Math.Abs(portion - 100) < 0.1) return "100 г";
            if (Math.Abs(portion - 1) < 0.1) return "г";
            return $"порция ({_product.ServingQuantity} г)";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
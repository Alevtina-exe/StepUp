using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using System;
using System.Timers;
using Path = Microsoft.Maui.Controls.Shapes.Path;
using Timer = System.Timers.Timer;

namespace WeightTracker.Models;
public class ColorChanger
{
    private Path targetPath;
    private Label targetLabel;
    private Label targetLabel2;
    private int duration = 2000; 
    private int steps = 100; 
    private int currentStep = 0;
    private double _progress;

    public ColorChanger(Path path, Label label, Label label2, double progress)
    {
        targetPath = path;
        targetLabel = label;
        targetLabel2 = label2;
        _progress = progress;
    }
    public static Color GetInterpolatedColor(float progress)
    {
        Color[] colors = { Colors.Green, Colors.Yellow, Colors.Orange, Colors.Red };
        int index = (int)(progress * (colors.Length - 1));
        float localProgress = (progress * (colors.Length - 1)) - index;

        Color start = colors[index];
        Color end = colors[Math.Min(index + 1, colors.Length - 1)];

        return new Color(
            start.Red + localProgress * (end.Red - start.Red),
            start.Green + localProgress * (end.Green - start.Green),
            start.Blue + localProgress * (end.Blue - start.Blue)
        );
    }
    public static (double x, double y) GetPointOnCircle(double progress)
    {
        double radius = 70, centerX = 70, centerY = 70;
        double angleRadians = (Math.PI * 2 * progress) - (Math.PI / 2);
        double x = (float)(centerX + radius * Math.Cos(angleRadians));
        double y = (float)(centerY + radius * Math.Sin(angleRadians));
        return (x, y);
    }
    public async Task AnimateColor()
    {
        double progress = (double)currentStep / steps;
        if (currentStep > steps || progress > _progress || 
            DayResult.CurrentDay.CaloriePlan <= DayResult.CurrentDay.KcalRes)
        {
            if(currentStep == 0)
            {
                targetPath.IsVisible = false;
                targetPath.Data = new PathGeometry
                {
                    Figures = new PathFigureCollection
                    {
                        new PathFigure
                        {
                            StartPoint = new Point(70, 0),                            
                            Segments = new PathSegmentCollection
                            {
                                new ArcSegment
                                {
                                    Point = new Point(70, 0),
                                    Size = new Size(70, 70),
                                    IsLargeArc = false,
                                    SweepDirection = SweepDirection.CounterClockwise
                                }
                            }
                        }
                    }
                };
                targetLabel.TextColor = Colors.Green;
                targetLabel2.TextColor = Colors.Green;
            }
            targetLabel.Text = DayResult.CurrentDay.KcalRes.ToString();
            return;
        }
        if (targetPath.Data is EllipseGeometry)
        {
            targetPath.Data = new PathGeometry
            {
                Figures = new PathFigureCollection
            {
                new PathFigure
                {
                    StartPoint = new Point(70, 0),
                    Segments = new PathSegmentCollection
                    {
                        new ArcSegment
                        {
                            Point = new Point(70, 0),
                            Size = new Size(70, 70),
                            IsLargeArc = false,
                            SweepDirection = SweepDirection.CounterClockwise
                        }
                    }
                }
            }
            };
        }
        PathFigure? figure = null;
        if (targetPath.Data is PathGeometry geo)
        {
            figure = geo.Figures[0];
            if (figure != null)
            { 
                ArcSegment? arc = figure.Segments.OfType<ArcSegment>().FirstOrDefault();
                if (arc != null)
                {
                    if(currentStep == 0)
                    {
                        targetPath.IsVisible = false;
                    }
                    if(currentStep == 1)
                    {
                        arc.SweepDirection = SweepDirection.Clockwise;
                        targetPath.IsVisible = true;
                    }
                    if (currentStep == steps)
                    {
                        targetPath.Data = new EllipseGeometry
                        {
                            Center = new Point(70, 70),
                            RadiusX = 70,
                            RadiusY = 70
                        };
                    }
                    else
                    {
                        (double x, double y) point = GetPointOnCircle(progress);
                        if (progress > 0.5 && progress != 1)
                        {
                            arc.IsLargeArc = true;
                        }
                        else
                        {
                            arc.IsLargeArc = false;
                        }
                        arc.Point = new Point(point.x, point.y);
                    }
                }
            }
        }
        Color currentColor = GetInterpolatedColor((float)progress);
        targetPath.Stroke = new SolidColorBrush(currentColor);
        targetLabel2.TextColor = currentColor;
        targetLabel.TextColor = currentColor;
        targetLabel.Text = ((int)((double)currentStep / (_progress * steps) * DayResult.CurrentDay.KcalRes)).ToString();
        await Task.Delay(duration / steps);
        currentStep++;
        await AnimateColor();
    }
}

namespace StepUp.Presentation.Models;

public readonly record struct DoubleChartPoint(DateTime Date, double? Value);
public readonly record struct IntChartPoint(DateTime Date, int? Value);
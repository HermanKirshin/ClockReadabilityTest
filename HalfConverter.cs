using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace ClockReadabilityTest;

public enum Calculation
{
    Default,
    Inverted,
    MultiplySin,
    MultiplyCos
}

public class HalfConverter(Calculation calculation) : MarkupExtension, IMultiValueConverter, IValueConverter
{
    public static readonly AttachedProperty<double> AngleProperty = AvaloniaProperty.RegisterAttached<HalfConverter, Control, double>("Angle", inherits: true);

    public static double GetAngle(Control control) => control.GetValue(AngleProperty);
    public static void SetAngle(Control control, double value) => control.SetValue(AngleProperty, value);
    
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var doubleValues = values.Select(x => x is UnsetValueType ? 0 : System.Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();
        
        if (values.Count < 1)
            return null;

        var doubleValue = doubleValues[0];
        var angle = doubleValues.Length >= 2 ? doubleValues[1] : 0;
        var multiplier = doubleValues.Length >= 3 ? doubleValues[2] : 1;
        
        return calculation switch
        {
            Calculation.MultiplySin => multiplier * Math.Sin(angle * Math.PI / 180) * doubleValue / 2,
            Calculation.MultiplyCos => multiplier * Math.Cos(angle * Math.PI / 180) * doubleValue / 2,
            Calculation.Default => doubleValue / 2,
            Calculation.Inverted => doubleValue / -2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double doubleValue)
            return value;
        return calculation switch
        {
            Calculation.Default => doubleValue / 2,
            Calculation.Inverted => doubleValue / -2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

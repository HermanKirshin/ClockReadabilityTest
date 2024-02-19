using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace ClockReadabilityTest;

public class MultiplyConverter : MarkupExtension, IMultiValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var doubleValues = values.Select(x => x is UnsetValueType ? 0 : System.Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();
        return doubleValues.Aggregate((x, y) => x * y);
    }
}
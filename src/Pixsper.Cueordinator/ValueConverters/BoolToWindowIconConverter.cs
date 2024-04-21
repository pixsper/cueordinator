using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace Pixsper.Cueordinator.ValueConverters;

public class BoolToWindowIconConverter : IValueConverter
{
    public WindowIcon? FalseIcon { get; set; }
    public WindowIcon? TrueIcon { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? TrueIcon : FalseIcon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
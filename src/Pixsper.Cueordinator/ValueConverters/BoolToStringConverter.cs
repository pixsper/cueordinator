using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Pixsper.Cueordinator.ValueConverters;

internal class BoolToStringConverter : IValueConverter
{
    public static readonly BoolToStringConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool bValue)
        {
            if (parameter is string sParameter)
            {
                var options = sParameter.Split(';');
                if (options.Length != 2)
                    throw new ArgumentException("Parameter must have two options separated by a semicolon",
                        nameof(parameter));

                return bValue ? options[1] : options[0];
            }
            else
            {
                return bValue ? "True" : "False";
            }
        }
        else
        {
            return AvaloniaProperty.UnsetValue;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RepairTracking.Helpers;

public class PasswordCharConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool and true ? '\0' : '•';
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}
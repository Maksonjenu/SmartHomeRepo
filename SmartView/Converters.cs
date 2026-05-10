using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartView;

/// <summary>
/// Конвертирует bool в Visibility (true -> Visible, false -> Collapsed)
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is Visibility v && v == Visibility.Visible);
    }
}

/// <summary>
/// Конвертирует bool в Visibility (true -> Collapsed, false -> Visible) - обратный
/// </summary>
public class BoolInverseToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && !b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is Visibility v && v == Visibility.Collapsed);
    }
}

/// <summary>
/// Конвертирует null/не null в Visibility (null -> Collapsed, not null -> Visible)
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертирует пустую строку в Visibility (empty/null -> Collapsed, not empty -> Visible)
/// </summary>
public class StringEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (!string.IsNullOrEmpty(value?.ToString())) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

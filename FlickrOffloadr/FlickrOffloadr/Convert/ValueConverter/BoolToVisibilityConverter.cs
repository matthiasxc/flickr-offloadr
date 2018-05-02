using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FlickrOffloadr.Convert.ValueConverter
{
    public class BoolToVisibilityConverter : DependencyObject, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = value is bool && (bool)value;

            if (boolValue)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }

    public class ReverseBoolToVisibilityConverter : DependencyObject, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = value is bool && (bool)value;

            if (boolValue)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}

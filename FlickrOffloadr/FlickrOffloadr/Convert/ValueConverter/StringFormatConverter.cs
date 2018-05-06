using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FlickrOffloadr.Convert.ValueConverter
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = parameter as string;

            if (!String.IsNullOrEmpty(format))
            {
                format = format.Replace("\\", string.Empty);
                string formatSample = String.Format(format, value);
                return formatSample;

            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

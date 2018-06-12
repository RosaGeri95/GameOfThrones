using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace GameOfThrones.Converters
{
    /// <summary>
    /// Converts an empty string to the "-" character, needed so the user can see that a data is not specified by the web API
    /// </summary>
    public class EmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a String");
            }

            string data = (string)value;
            if ( data == "")
            {
                data = "-";
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a String");
            }
            string data = (string)value;
            if (data == "-")
            {
                data = "";
            }
            return data;
        }
    }
}

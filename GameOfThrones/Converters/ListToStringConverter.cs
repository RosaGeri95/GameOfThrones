using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace GameOfThrones.Converters
{
    /// <summary>
    /// Converts a list of string to a single string, needed for binding certain field values
    /// </summary>
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is List<string>)
            {
                List<string> temp = (List<string>)value;

                if (temp.Count == 0)
                {
                    return "-";
                }

                StringBuilder sb = new StringBuilder();
                for( int i = 0; i < temp.Count; i++)
                {
                    sb.Append(temp[i]);
                    if (i != temp.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                return sb.ToString();
            }
            else
            {
                throw new Exception("wrong type");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

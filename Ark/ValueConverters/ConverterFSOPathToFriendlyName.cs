using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.IO;

namespace Ark.ValueConverters
{
    class ConverterFSOPathToFriendlyName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value.ToString();
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                return di.Name;
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                return fi.Name;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace MothershipApp.Converters
{
    public class ResponseStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AppServiceResponseStatus)
            {
                switch ((AppServiceResponseStatus)value)
                {
                    case AppServiceResponseStatus.Success:
                        return Colors.Green;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Framework.Library.Filter
{
    public class DateFilter : IsoDateTimeConverter
    {
        public DateFilter()
        {
            // DateTimeStyles=DateTimeStyles.no
          //   Culture = CultureInfo.InvariantCulture;
           //  DateTimeFormat = "dd-MM-yyyy hh:mm";
            DateTimeFormat = "yyyy/MM/dd hh:mm";
        }
    }
}

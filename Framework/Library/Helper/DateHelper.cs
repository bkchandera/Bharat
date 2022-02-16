using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Framework.Library.Helper
{
    public static class DateHelper
    {
        public static string CurrentTimestamp(this DateTime Value)
        {
            return Value.ToString("yymmddHHmmssffff");
        }
        public static string CurrentTimestamp(this string Value)
        {
            return Value+"_"+DateTime.Now.ToString("yyMMddHHmmssffff");
        }
        public static DateTime CurrentDate()
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = culture;
            return DateTime.Now;
        }
        public static DateTime ToDateTime(string date_value)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            //Thread.CurrentThread.CurrentCulture = culture;

            return DateTime.ParseExact(date_value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
           
        }
    }
}

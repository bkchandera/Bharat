using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Library.Validator
{
    public static class DateValidator
    {
        public static bool LessThanOrEqual(DateTime given_date)
        {
            if (given_date.Date <= DateTime.Today.Date)
                return true;
            else
                return false;
        }

        public static bool GreaterThanOrEqual(DateTime given_date)
        {
            if (given_date.Date >= DateTime.Today.Date)
                return true;
            else
                return false;
        }
        public static bool GreaterThanOrEqual(DateTime? given_date)
        {          
            if (given_date.Value >= DateTime.Today.Date)
                return true;
            else
                return false;
        }
    }
}

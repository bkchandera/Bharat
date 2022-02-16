using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Library.Helper
{
    public static partial class MathHelper
    {
        public static decimal TruncateValue(decimal value, int trucnate_digit = 0)
        {
            decimal factor = (decimal)Math.Pow(10, trucnate_digit);
            return Math.Truncate(factor * value) / factor;
        }

        public static decimal RoundValue(decimal value, int round_value = 0)
        {
            return Math.Round(value, round_value);
        }
    }
}

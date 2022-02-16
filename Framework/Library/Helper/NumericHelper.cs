using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Library.Helper
{
    public static class NumericHelper
    {
        public static int RandomNumber(int lenght=9999)
        {
            Random rand = new Random();
            return rand.Next(lenght);
        }
    }
}

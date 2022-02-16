using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Framework.Library.Helper
{
    public static class StringHelper
    {
       
       
        public static string ToTitleCase(this string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str);
        }
        public static IEnumerable<T> NotEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null;
        }
        public static bool NotEmpty(this string str)
        {
            return str != null && str != "0" && str.Trim() != "";
        }
        public static bool Empty(this string str)
        {
            return str == null || str == "0" || str.Trim() == "";
        }
        public static bool IsEmpty(this string str)
        {
            return str == null || str == "0" || str.Trim() == "";
        }

        public static string[] ReplaceSubstring(this string[] str,char from='.')
        {
            return str.Select(x => x.Substring(x.IndexOf(from) + 1).Trim()).ToArray();
        }
        public static string ReplaceSubstring(this string str, char from = '.')
        {
            return str.Substring(str.IndexOf(from) + 1).Trim();
        }
        public static string ReplaceSubstring(this string str, string from)
        {
            return str.Substring(str.IndexOf(from) + 1).Trim();
        }
        public static string[] ReplaceSubstring(this string[] str, string from)
        {
            return str.Select(x => x.Substring(x.IndexOf(from)==-1?0:x.IndexOf(from)+from.Length).Trim()).ToArray();
        }

        public static string ModelName(this string TableName)
        {
            return TableName.Replace("_"," ").ToTitleCase().Replace(" ", "");
        }
        public static string ValidatorName(this string ModelName)
        {
            return ModelName + "Validator";
        }
        public static bool IsBase64(this string str,out byte[] output)
        {
            output = null;
            try
            {
                output = Convert.FromBase64String(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}

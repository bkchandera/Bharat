using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library.Helper
{
    public static class ValidationHelper
    {
        public static Dictionary<string,string> GetErrors(this FluentValidation.Results.ValidationResult result)
        {
            return result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
        }
        public static Dictionary<string, string> GetErrors(this string PropertyName, string ErrorMessage)
        {
            return new Dictionary<string, string>() {{ PropertyName , ErrorMessage }};
        }
        public static string GetErrors(this FluentValidation.Results.ValidationResult result,bool flag)
        {
            return string.Join(',', result.Errors.Select(e => $"{e.PropertyName}:{e.ErrorMessage}").ToArray<string>());
        }
    }
}

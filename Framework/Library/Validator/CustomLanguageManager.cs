using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Framework.Library.Validator
{
    public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
    {
        public CustomLanguageManager()
        {
            //  AddTranslation("en", "NotNullValidator", "required");
            // AddTranslation("en", "NotEmptyValidator", "not-empty");
            //  AddTranslation("en", "LengthValidator", "length");
        }
        public override string GetString(string key, CultureInfo culture = null)
        {
            switch (key)
            {
                /*for more validator keys references link https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Resources/Languages/EnglishLanguage.cs */

                case "ExactLengthValidator":
                    return "validLength~{MaxLength}";
                case "LengthValidator":
                    return "validLengthRange~{MinLength}~{MaxLength}";
                case "NotEmptyValidator":
                    return "validRequire";
                case "numeric":
                    return "validNumber";
                case "require":
                    return "validRequire";
                case "name":
                    return "validAlpha~.-";
                case "PredicateValidator":
                    return "Duplicate Key";
                case "GreaterThanOrEqualValidator":
                    return "validGreaterThenOrEqual~{ComparisonValue}";
                case "GreaterThanValidator":
                    return "validGreaterThen~{ComparisonValue}";
                case "LessThanOrEqualValidator":
                    return "validLessThenOrEqual~{ComparisonValue}";
                case "LessThanValidator":
                    return "validLessThan~{ComparisonValue}";
                case "validDecimal":
                    return "Decimal value is out of range.";
                case "InclusiveBetweenValidator":
                    return "'{PropertyName}' must be between {From} and {To}. You entered {Value}.";
                case "ExclusiveBetweenValidator":
                    return "'{PropertyName}' must be between {From} and {To} (exclusive). You entered {Value}.";
                case "applicationNumber":
                    return "invalid_application_number";
                default:
                    return string.Empty;
            }
        }

        //public bool Enabled { get; set; }
        //public CultureInfo Culture { get; set; }
    }
}

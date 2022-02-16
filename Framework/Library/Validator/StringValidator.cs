using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace Framework.Library.Validator
{
    public static class StringValidator
    {
        public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = new Regex(@"^[a-zA-Z]+[a-zA-Z\s.-]*$");
            return ruleBuilder.NotEmpty().NotNull().Matches(regex).WithErrorCode("name");
        }
        public static IRuleBuilderOptions<T, string> NameSpecial<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = new Regex(@"^[a-zA-Z0-9]+[a-zA-Z0-9()\s.-]*$");
            return ruleBuilder.NotEmpty().NotNull().Matches(regex).WithErrorCode("name");
        }
        public static IRuleBuilderOptions<T, string> AlphaNumeric<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]*$");
            return ruleBuilder.NotEmpty().NotNull().Matches(regex).WithErrorCode("name");
        }

        public static IRuleBuilderOptions<T, string> Require<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().NotNull().WithErrorCode("require");
        }
        public static IRuleBuilderOptions<T, DateTime> Require<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().NotNull().WithErrorCode("require");
        }
        public static IRuleBuilderOptions<T, DateTime?> Require<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().NotNull().WithErrorCode("require");
        }

        public static IRuleBuilderOptions<T, int> Require<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().NotNull().WithErrorCode("require");
        }
        public static IRuleBuilderOptions<T, decimal> Require<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder.NotEmpty().NotNull().WithErrorCode("require");
        }
        public static IRuleBuilderOptions<T, bool> Require<T>(this IRuleBuilder<T, bool> ruleBuilder)
        {
            return ruleBuilder.NotNull().WithErrorCode("require");
        }

        public static IRuleBuilderOptions<T, string> StringLength<T>(this IRuleBuilder<T, string> ruleBuilder, int exactLength)
        {
            return ruleBuilder.NotNull().Length(exactLength).WithErrorCode("ExactLengthValidator");
        }

        public static IRuleBuilderOptions<T, string> StringLength<T>(this IRuleBuilder<T, string> ruleBuilder, int minLength, int maxLength)
        {
            return ruleBuilder.NotNull().Length(minLength, maxLength).WithErrorCode("LengthValidator");
        }
        public static IRuleBuilderOptions<T, int> Require<T>(this IRuleBuilder<T, int> ruleBuilder, string withMessage = null)
        {
            return ruleBuilder
                    .NotEmpty().WithMessage(withMessage == null ? "require" : withMessage)
                    .NotNull().WithMessage(withMessage == null ? "require" : withMessage);
        }

        public static IRuleBuilderOptions<T, string> ApplicationNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = new Regex(@"^[a-zA-Z]{1}[0-9]{1,7}$");
            return ruleBuilder.NotEmpty().NotNull().Matches(regex).WithErrorCode("applicationNumber");
        }
        public static IRuleBuilderOptions<T, string> PanCard<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regex = new Regex(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$");            
            return ruleBuilder.Matches(regex).WithErrorCode("notvalid_pancard");
        }
    }
}

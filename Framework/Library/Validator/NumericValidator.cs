using FluentValidation;
using Framework.Models;

namespace Framework.Library.Validator
{
    public static class NumericValidator
    {
        public static IRuleBuilderOptions<T, int> Numeric<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(0).WithErrorCode("numeric");
        }

        public static IRuleBuilderOptions<T, string> Numeric<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Matches("^[0-9]+$").WithErrorCode("numeric").Matches("^(?!0*$).*$").WithErrorCode("numeric");
        }

        /// <summary>
        /// Set decimal validator
        /// </summary>
        /// <typeparam name="T">Generic Model Type</typeparam>
        /// <param name="ruleBuilder">RuleBuilder</param>
        /// <param name="maxValue">Maximum value</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<T, decimal> IsDecimal<T>(this IRuleBuilder<T, decimal> ruleBuilder, decimal maxValue = decimal.MaxValue, int decimals = 2) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new DecimalPropertyValidator<T>(maxValue, decimals));
        }

        /// <summary>
        /// Set decimal validator
        /// </summary>
        /// <typeparam name="T">Generic Model Type</typeparam>
        /// <param name="ruleBuilder">RuleBuilder</param>
        /// <param name="maxValue">Maximum value</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<T, string> IsDecimal<T>(this IRuleBuilder<T, string> ruleBuilder, decimal maxValue = decimal.MaxValue, int decimals = 2) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new DecimalPropertyValidator<T>(maxValue, decimals));
        }
    }
}

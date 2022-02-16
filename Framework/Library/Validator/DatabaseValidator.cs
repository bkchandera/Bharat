using FluentValidation;
using Framework.CustomDataType;
using Framework.Models;
using System;
using System.Collections.Generic;

namespace Framework.Library.Validator
{
    public static class DatabaseValidator
    {

        public static IRuleBuilderOptions<T, string> Unique<T>(this IRuleBuilder<T, string> ruleBuilder) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueValidator<T>());
        }
        public static IRuleBuilderOptions<T, string> Unique<T>(this IRuleBuilder<T, string> ruleBuilder, string Field) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueValidatorV2<T>(Field));
        }
        public static IRuleBuilderOptions<T, int> Unique<T>(this IRuleBuilder<T, int> ruleBuilder, string Field) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueValidatorV2<T>(Field));
        }
        public static IRuleBuilderOptions<T1, T2> Unique<T1, T2>(this IRuleBuilder<T1, T2> ruleBuilder, List<ConditionParameter> Condition, string Field) where T1 : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueApplicability<T1>(Condition, Field));
        }
        public static IRuleBuilderOptions<T, string> Unique<T>(this IRuleBuilder<T, string> ruleBuilder, QueryParam Query) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueDepended<T>(Query));
        }
        public static IRuleBuilderOptions<T, DateTime> Unique<T>(this IRuleBuilder<T, DateTime> ruleBuilder, QueryParam Query) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueDepended<T>(Query));
        }
        public static IRuleBuilderOptions<T, string> Range<T>(this IRuleBuilder<T, string> ruleBuilder, QueryParam Query) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new RangeValidator(Query));
        }
        public static IRuleBuilderOptions<T, int> Range<T>(this IRuleBuilder<T, int> ruleBuilder, QueryParam Query) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new RangeValidator(Query));
        }
        public static IRuleBuilderOptions<T, DateTime> Range<T>(this IRuleBuilder<T, DateTime> ruleBuilder, QueryParam Query) where T : BaseModel
        {
            return ruleBuilder.SetValidator(new RangeValidator(Query));
        }
        /// <summary>
        /// this validator is use for check db or query level validation
        /// </summary>
        /// <typeparam name="T1">Model Object</typeparam>
        /// <typeparam name="T2">Model Object</typeparam>
        /// <param name="ruleBuilder">Rule builder</param>
        /// <param name="queryParams">Condition as Query Params</param>
        /// <param name="Field">Pass Field to check</param>
        /// <param name="returnCheck">if returnCheck true then it will check data is present or not. </param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T1, T2> PaymentCycle<T1, T2>(this IRuleBuilder<T1, T2> ruleBuilder, int status) where T1 : BaseModel
        {
            return ruleBuilder.SetValidator(new UniqueApplicabilityV2<T1>(status));
        }
        public static IRuleBuilderOptions<T1, T2> QueryBasedValidator<T1, T2>(this IRuleBuilder<T1, T2> ruleBuilder, QueryParam Query) where T1 : BaseModel
        {
            return ruleBuilder.SetValidator(new QueryBasedValidator<T1>(Query));
        }

        public static IRuleBuilderOptions<T1, T2> IsValidEffectiveDate<T1, T2>(this IRuleBuilder<T1, T2> ruleBuilder) where T1 : BaseModel
        {
            return ruleBuilder.SetValidator(new IsValidEffectiveDate<T1>());

        }

        public static IRuleBuilderOptions<T1, T2> CheckHierarchyValidator<T1, T2>(this IRuleBuilder<T1, T2> ruleBuilder) where T1 : BaseModel
        {
            return ruleBuilder.SetValidator(new CheckHierarchyValidator<T1>());

        }
    }
}

using FluentValidation.Validators;
using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Library.Validator
{
    public class CheckHierarchyValidator<T> : PropertyValidator where T : BaseModel
    {
        QueryParam _queryParams;

        public CheckHierarchyValidator() : base("{ValidationMessage}")
        {
            _queryParams = new QueryParam
            {
                Sp = "check_valid_hierarchy_data",
                Where = new List<ConditionParameter>()
                {
                    new ConditionParameter{PropertyName="p_bmc_code"},
                    new ConditionParameter{PropertyName="p_mpp_code"},
                    new ConditionParameter{PropertyName="p_member_code"},
                }
            };
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            _queryParams.Where.ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            bool validation_response = Repository.Find<bool>(_queryParams);
            if (validation_response)
            {
                context.MessageFormatter.AppendArgument("ValidationMessage", "master_data_hierarchy_is_not_perfect");
                return false;
            }
            else
                return true;
        }
    }
}

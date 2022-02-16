using FluentValidation.Validators;
using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Library.Validator
{
    class UniqueApplicability<T> : PropertyValidator where T : BaseModel
    {
        List<ConditionParameter> _Condition;
        string _Field;
        public UniqueApplicability(List<ConditionParameter> Condition, string Field) : base("{ValidationMessage}")
        {
            _Condition = Condition;
            _Field = Field;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {

            DBRepository Repository = new DBRepository();
            //_Condition.Add(new ConditionParameter { PropertyName = _Field, PropertyValue = context.Instance.GetPropertyValue(_Field) });
            ModelInfo model = context.Instance.GetProperty("model_operation");
            if (model.PropertyValue == "update")
            {
                model = context.Instance.GetPrimaryKey();
                _Condition.Add(new ConditionParameter { Operator = "!=", PropertyName = model.PropertyName, PropertyValue = model.PropertyValue });
            }

            _Condition.ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));

            QueryParam Query = new QueryParam
            {
                Fields = _Field,
                Table = typeof(T).GetTableName(),
                Where = _Condition
            };
            string s = Repository.Find<string>(Query);
            if (s == null || s == "")
            {
                return true;
            }
            else
            {
                //  context.MessageFormatter.BuildMessage($"Module_code({context.Instance.GetPropertyValue("module_code")}) to given {_Field}:{s}");
                context.MessageFormatter.AppendArgument("ValidationMessage", $"Module_code({context.Instance.GetPropertyValue("module_code")}) to given {_Field}:{s}");
                return false;
            }


        }
    }


    class UniqueDepended<T> : PropertyValidator where T : BaseModel
    {
        QueryParam _Query;
        public UniqueDepended(QueryParam Query) : base("{ValidationMessage}")
        {
            _Query = Query;

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            //   _Query.Where.ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            if (_Query.Where == null)
                _Query.Where = new List<ConditionParameter>();
            _Query.Where.ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            _Query.Where.Add(new ConditionParameter { PropertyName = $"{_Query.Table}.{context.PropertyName}", PropertyValue = context.PropertyValue });

            ModelInfo model = context.Instance.GetProperty("model_operation");
            if (model.PropertyValue == "update")
            {
                model = context.Instance.GetPrimaryKey();
                _Query.Where.Add(new ConditionParameter { Operator = "!=", PropertyName = $"{context.Instance.GetType().GetTableName()}.{model.PropertyName}", PropertyValue = model.PropertyValue });
            }
            string s = Repository.Find<string>(_Query);
            if (s == null || s == "")
            {
                return true;
            }
            else
            {
                //  context.MessageFormatter.BuildMessage($"Module_code({context.Instance.GetPropertyValue("module_code")}) to given {_Field}:{s}");
                context.MessageFormatter.AppendArgument("ValidationMessage", $"'{context.PropertyValue}' already used in '{_Query.Fields}-{s}'");
                return false;
            }


        }
    }

    /// <summary>
    /// this unique Applicability V2 check whether applicability available then return true either false
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueApplicabilityV2<T> : PropertyValidator where T : BaseModel
    {
        QueryParam _queryParams;
        int _status;
        public UniqueApplicabilityV2(int status) : base("{ValidationMessage}")
        {
            _status = status;
            _queryParams = new QueryParam
            {
                Sp = "process_check_payment_cycle_lock_applicability",
                Where = new List<ConditionParameter>()
                {
                    new ConditionParameter{PropertyName="p_bmc_code"},
                    new ConditionParameter{PropertyName="p_applicability_for"},
                    new ConditionParameter{PropertyName="p_lock_for"},
                    new ConditionParameter{PropertyName="p_collection_date"}
                }
            };
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            _queryParams.Where.ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            _queryParams.Where.Add(new ConditionParameter { PropertyName = "p_status", PropertyValue = _status });
            int cnt = Repository.Find<int>(_queryParams);           
            if ((_status == 0 || _status == 2) && cnt == 0)
            {
                context.MessageFormatter.AppendArgument("ValidationMessage", "cycle_not_applied");
                return false;
            }
            else if ((_status == 1 || _status == 2) && cnt == -1)
            {
                context.MessageFormatter.AppendArgument("ValidationMessage", "cycle_locked");
                return false;

            }
            else
                return true;
        }
    }

    public class IsValidEffectiveDate<T> : PropertyValidator where T : BaseModel
    {
        QueryParam _queryParams;
        
        public IsValidEffectiveDate() : base("{ValidationMessage}")
        {
            _queryParams = new QueryParam
            {
                Sp = "check_valid_effective_date",
                Where = new List<ConditionParameter>()
                {
                    new ConditionParameter{PropertyName="p_module_name"},
                    new ConditionParameter{PropertyName="p_module_code"},
                    new ConditionParameter{PropertyName="p_current_date"},
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
                context.MessageFormatter.AppendArgument("ValidationMessage", "effective_date_is_invalid");
                return false;
            }
            else
                return true;
        }
    }
}

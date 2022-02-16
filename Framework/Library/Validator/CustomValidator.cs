using FluentValidation.Validators;
using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Library.Validator
{
    //public class RuleFunction<T> where T : class
    //{
    //    public bool Unique(T item)
    //    {

    //        IRepository<T> Repository;
    //        Repository = new BaseRepository<T>();
    //        // if (Repository.Count(new Dictionary<string, dynamic> { { "state_code", "Gujrat" } }) > 0)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}

    public class UniqueValidator<T> : PropertyValidator where T : BaseModel
    {
        public UniqueValidator() : base("validUnique") { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            List<ConditionParameter> condition = new List<ConditionParameter>
            {
                new ConditionParameter { PropertyName = context.PropertyName, PropertyValue = context.PropertyValue }
            };
            ModelInfo model = context.Instance.GetProperty("model_operation");

            if (model.PropertyValue == "update")
            {
                model = context.Instance.GetPrimaryKey();
                condition.Add(new ConditionParameter { Operator = "!=", PropertyName = model.PropertyName, PropertyValue = model.PropertyValue });
            }
            QueryParam Query = new QueryParam
            {
                Where = condition
            };
            if (Repository.Count<T>(Query) > 0)
            {
                return false;
            }
            return true;
        }


        //var dictionary = ToDictionary<string>(x);
        //dictionary.TryGetValue(pk, out pkvalue);
        //public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        //{
        //    var json = JsonConvert.SerializeObject(obj);
        //    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
        //    return dictionary;
        //}

    }
    public class UniqueValidatorV2<T> : PropertyValidator where T : BaseModel
    {
        private string _Field;
        public UniqueValidatorV2(string Field) : base("validUnique")
        {
            _Field = Field;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            List<ConditionParameter> condition = new List<ConditionParameter>
            {
                new ConditionParameter { PropertyName = _Field, PropertyValue = context.Instance.GetPropertyValue(_Field) },
                new ConditionParameter { PropertyName = context.PropertyName, PropertyValue = context.PropertyValue }
            };
            ModelInfo model = context.Instance.GetProperty("model_operation");

            if (model.PropertyValue == "update")
            {
                model = context.Instance.GetPrimaryKey();
                condition.Add(new ConditionParameter { Operator = "!=", PropertyName = model.PropertyName, PropertyValue = model.PropertyValue });
            }
            QueryParam Query = new QueryParam
            {
                Where = condition
            };
            if (Repository.Count<T>(Query) > 0)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Decimal validator
    /// </summary>
    public class DecimalPropertyValidator<T> : PropertyValidator where T : BaseModel
    {
        public decimal _maxValue;
        public int _decimals;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxValue">Maximum value</param>
        ///<param name="decimals">The number of decimal places in the return value.</param>
        public DecimalPropertyValidator(decimal maxValue, int decimals) : base("validDecimal")
        {
            _maxValue = maxValue;
            _decimals = decimals;
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        /// <param name="context">Validation context</param>
        /// <returns>Result</returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (decimal.TryParse(context.PropertyValue.ToString(), out decimal value))
                return Math.Round(value, _decimals) < _maxValue;

            return false;
        }
    }
    public class RangeValidator : PropertyValidator
    {
        QueryParam _Query;
        public RangeValidator(QueryParam Query) : base("{ValidationMessage}")
        {
            _Query = Query;
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            //   _Query.Where.ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            if (_Query.Where == null)
                _Query.Where = new List<ConditionParameter>();
            _Query.Where.Where(x => x.PropertyName != null && x.PropertyName != "").ToList().ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            if (_Query.DynemicParam != null)
            {
                Dictionary<string, dynamic> new_param = new Dictionary<string, dynamic>();
                foreach (KeyValuePair<string, dynamic> param in _Query.DynemicParam)
                {
                    new_param[param.Key] = context.Instance.GetPropertyValue(param.Key.Replace('@', ' ').Trim());
                }
                _Query.DynemicParam = new_param;
            }

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
                context.MessageFormatter.AppendArgument("ValidationMessage", $"range_conflict~'{context.PropertyValue}' ");
                return false;
            }
        }
    }
}

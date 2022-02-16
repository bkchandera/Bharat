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

    public class QueryBasedValidator<T> : PropertyValidator where T : BaseModel
    {
        QueryParam _queryParams;
      
        public QueryBasedValidator(QueryParam query) : base("{ValidationMessage}")
        {

            _queryParams = query;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DBRepository Repository = new DBRepository();
            _queryParams.Where.Where(x=>x.direct_condition.Trim()=="n").ToList().ForEach(a => a.PropertyValue = context.Instance.GetPropertyValue(a.PropertyName));
            if (_queryParams.DynemicParam != null)
            {
                Dictionary<string, dynamic> new_param = new Dictionary<string, dynamic>();
                foreach (KeyValuePair<string, dynamic> param in _queryParams.DynemicParam)
                {
                    new_param[param.Key] = context.Instance.GetPropertyValue(param.Key.Replace('@', ' ').Trim());
                }
                _queryParams.DynemicParam = new_param;
            }

            int cnt = Repository.Find<int>(_queryParams);            
            if (cnt == 0)
            {
                 return true;
            }
            else 
            {
                context.MessageFormatter.AppendArgument("ValidationMessage", "already exist"); return false;

            }
        }
    }
}

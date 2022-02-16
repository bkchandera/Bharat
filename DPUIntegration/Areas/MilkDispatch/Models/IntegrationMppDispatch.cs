using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using FluentValidation;
using Framework.CustomDataType;
using Framework.Models;
using Framework.Library.Validator;

namespace DPUIntegration.Areas.MilkDispatch.Models
{
    [Table("integration_mpp_dispatch")]
    public class IntegrationMppDispatch : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int mpp_dispatch_code { get; set; }
        public string mpp_code { get; set; }
        public string bmc_code { get; set; }
        public DateTime transaction_date { get; set; }
        public DateTime collection_date { get; set; }
        public string shift_code { get; set; }
        public string milk_type { get; set; }
        public int sample_no { get; set; }
        public decimal qty { get; set; }
        public decimal fat { get; set; }
        public decimal snf { get; set; }
        public decimal water { get; set; }
        public DateTime sample_time { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
    }


    public class IntegrationMppDispatchValidator : AbstractValidator<IntegrationMppDispatch>
    {
        public IntegrationMppDispatchValidator()
        {
            QueryParam Query = new QueryParam
            {
                Fields = "mpp_dispatch_code",
                Table = "integration_mpp_dispatch",
                Where = new List<ConditionParameter>
                {
                     new ConditionParameter{PropertyName="mpp_code"},
                     new ConditionParameter{PropertyName="bmc_code"},
                     new ConditionParameter{PropertyName="collection_date"},
                     new ConditionParameter{PropertyName="shift_code"},
                     new ConditionParameter{PropertyName="milk_type"},
                }
            };

            List<string> shift_code_condition = new List<string> { "E", "M" };
            RuleFor(d => d.shift_code).Require().Must(d => shift_code_condition.Contains(d))
                    .WithMessage("For Shift code only use: " + String.Join(",", shift_code_condition));

            List<string> milk_type_condition = new List<string> { "C", "B", "M" };
            RuleFor(d => d.milk_type).Require().Must(d => milk_type_condition.Contains(d))
                    .WithMessage("For Milk type only use: " + String.Join(",", milk_type_condition));

            RuleFor(d => d.sample_no).Require();
            RuleFor(d => d.fat).Require().InclusiveBetween(1, 16);
            RuleFor(d => d.snf).Require().InclusiveBetween(4, 22);
            RuleFor(d => d.qty).Require().GreaterThan(0);
            RuleFor(d => d.rate).Require().GreaterThan(0);
            RuleFor(d => d.amount).Require().GreaterThan(0);
            RuleFor(d => d.collection_date).Require().Unique(Query)
                .WithMessage("dispatch_already_exist");
            RuleFor(d => d.transaction_date).Require().Must(DateValidator.LessThanOrEqual)
               .WithMessage("future_date_is_not_allowed")
               .When(x => x.model_operation == "insert");
        }
    }
}

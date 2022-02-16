using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using FluentValidation;
using Framework.CustomDataType;
using Framework.Models;
using Framework.Library.Validator;

namespace DPUIntegration.Areas.MilkSummary.Models
{
    [Table("integration_shift_end_summary")]
    public class IntegrationMilkSummary : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int Id { get; set; }
        public string bmc_code { get; set; }
        public string mpp_code { get; set; }
        public DateTime transaction_date { get; set; }
        public DateTime collection_date { get; set; }
        public string shift_code { get; set; }
        public decimal qty { get; set; }
        public decimal total_amount { get; set; }
        public DateTime sample_date_time { get; set; }
        public int cow_total_count { get; set; }
        public int buffalo_total_count { get; set; }
        public int mix_total_count { get; set; }
        public int total_sample_count { get; set; }
    }

    public class IntegrationMilkSummaryValidator : AbstractValidator<IntegrationMilkSummary>
    {
        public IntegrationMilkSummaryValidator()
        { 
            List<string> shift_code_condition = new List<string> { "E", "M" };
            RuleFor(d => d.shift_code).Require().Must(d => shift_code_condition.Contains(d))
                    .WithMessage("For Shift code only use: " + String.Join(",", shift_code_condition));
            RuleFor(d => d.qty).Require().GreaterThan(0);
            RuleFor(d => d.total_amount).Require().GreaterThan(0);
            RuleFor(d => d.transaction_date).Require().Must(DateValidator.LessThanOrEqual)
               .WithMessage("future date is not allowed for transaction date")
               .When(x => x.model_operation == "insert");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using FluentValidation;
using Framework.CustomDataType;
using Framework.Models;
using Framework.Library.Validator;

namespace DPUIntegration.Areas.Acknowledgement.Models
{
    [Table("integration_transaction_acknowledgement")]
    public class IntegrationTxAcknowledgement : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int Id { get; set; }
        public string bmc_code { get; set; }
        public string mpp_code { get; set; }
        public DateTime transaction_date { get; set; }
        public DateTime download_date { get; set; }
        public string key_name { get; set; }
        public string key_value { get; set; }
        public bool download_success { get; set; }
    }

    public class IntegrationTxAcknowledgementValidator : AbstractValidator<IntegrationTxAcknowledgement>
    {
        public IntegrationTxAcknowledgementValidator()
        {

            RuleFor(d => d.transaction_date).Require().Must(DateValidator.LessThanOrEqual)
                   .WithMessage("future date is not allowed for transaction date")
                   .When(x => x.model_operation == "insert");
        }
    }
}
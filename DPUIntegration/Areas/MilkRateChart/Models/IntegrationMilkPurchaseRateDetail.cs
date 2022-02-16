using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Models;
using Dapper.Contrib.Extensions;

namespace DPUIntegration.Areas.MilkRateChart.Models
{
    [Table("integration_milk_purchase_rate_detail")]
    public class IntegrationMilkPurchaseRateDetail : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int purchase_rate_details_code { get; set; }
        public string purchase_rate_code { get; set; }
        public int milk_quality_type_code { get; set; }
        public int milk_type_code { get; set; }
        public decimal quality_param_one { get; set; }
        public decimal rtpl { get; set; }
        public decimal quality_param_two { get; set; }
        [Computed]
        public string module_code { get; set; }
        [Computed]
        public string module_name { get; set; }
        [Computed]
        public string rate_for { get; set; }
        [Computed]
        public DateTime? wef_date { get; set; }

        [Computed]
        public string bmc_code { get; set; }
        [Computed]
        public string device_id { get; set; }
        [Computed]
        public string hash_key { get; set; }

    }
}
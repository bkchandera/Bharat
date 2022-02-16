using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Framework.Models;

namespace DPUIntegration.Areas.MilkRateChart.Models
{
    [Table("integration_milk_purchase_rate_base")]
    public class IntegrationMilkPurchaseRateBase : BaseModel
    {
        [ExplicitKey]
        [Key]
        public int purchase_rate_based_code { get; set; }
        public string purchase_rate_code { get; set; }
        public int milk_quality_type_code { get; set; }
        public int milk_type_code { get; set; }
        public int quality_param_code { get; set; }
        public string start_range { get; set; }
        public string end_range { get; set; }
        public decimal kg_rate { get; set; }
        public int deduction_type { get; set; }
        public decimal fixed_point { get; set; }
        public int step { get; set; }
        public decimal value { get; set; }
        public int formula_master_code { get; set; }
        public string formula { get; set; }
        public int ref_type { get; set; }
        public string rate_type_name { get; set; }
        [Computed]
        public string milk_type_name { get; set; }
        [Computed]
        public string milk_quality_type_name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using FluentValidation;
using Framework.CustomDataType;
using Framework.Models;
using Framework.Library.Validator;

namespace DPUIntegration.Areas.MppConfiguration.Models
{
    [Table("integration_mpp_configuration")]
    public class IntegrationMppConfiguration : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int Id { get; set; }
        public string mpp_code { get; set; }
        public string mpp_name { get; set; }
        public string ref_code { get; set; }
        public string bmc_code { get; set; }
        public string bmc_name { get; set; }
        public int rate_flag { get; set; }
        public bool member_update { get; set; }
        public string startup_message { get; set; }
        public string quantity_mode { get; set; }
        public string buffalo_cutoff_price { get; set; }
        public string dpu_deviceId { get; set; }
        public string user_password { get; set; }
        public string supervisor_password { get; set; }
        public string admin_password { get; set; }
        public string default_milktype { get; set; }
        public string dispatch_default_milktype { get; set; }
        public bool is_auto_milk_type { get; set; }
        public decimal cutoff_fat { get; set; }
        public bool is_active { get; set; }
    }
}

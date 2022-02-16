using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DPUIntegration.Areas.Acknowledgement.Models
{
    public class TmpIntegrationTxAcknowledgement
    {
        public string bmc_code { get; set; }
        public DateTime transaction_date { get; set; }
        public List<TmpIntegrationTxAcknowledgementtx> acknowledgement_data { get; set; }
    }


    public class TmpIntegrationTxAcknowledgementtx
    {
        public string mpp_code { get; set; }
        public DateTime download_date { get; set; }
        public string key_name { get; set; }

        [StringLength(10)]
        public string rate_code { get; set; }
        public string milk_type { get; set; }
        public bool download_success { get; set; }
    }
}

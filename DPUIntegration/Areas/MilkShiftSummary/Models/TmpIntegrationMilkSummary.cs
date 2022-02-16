using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPUIntegration.Areas.MilkSummary.Models
{
    public class TmpIntegrationMilkSummary
    {
        public TmpIntegrationMilkSummary()
        {
            summary_data = new List<TmpIntegrationMilkSummaryTxn>();
        }
        public string bmc_code { get; set; }
        public DateTime transaction_date { get; set; }
        public DateTime collection_date { get; set; }
        public string shift_code { get; set; }
        public int data_count { get; set; }
        public List<TmpIntegrationMilkSummaryTxn> summary_data { get; set; }

    }

    public class TmpIntegrationMilkSummaryTxn
    {
        public string mpp_code { get; set; }
        public decimal qty { get; set; }
        public decimal total_amount { get; set; }
        public DateTime sample_date_time { get; set; }
        public int cow_total_count { get; set; }
        public int buffalo_total_count { get; set; }
        public int mix_total_count { get; set; }
        public int total_sample_count { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPUIntegration.Areas.MilkDispatch.Models
{
    public class TmpIntegrationMppDispatch
    {
        public TmpIntegrationMppDispatch()
        {
            dispatch_data = new List<TmpIntegrationMppDispatchTxn>();
        }
        public string bmc_code { get; set; }
        public DateTime transaction_date { get; set; }
        public DateTime collection_date { get; set; }
        public string shift_code { get; set; }
        public int data_count { get; set; }

        public List<TmpIntegrationMppDispatchTxn> dispatch_data { get; set; }
    }

    public class TmpIntegrationMppDispatchTxn
    {
        public string mpp_code { get; set; }
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
}
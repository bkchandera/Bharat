using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace DPUIntegration.Areas.MppCollection.Models
{
    public class TmpIntegrationMppCollection
    {
        public TmpIntegrationMppCollection()
        {
            collection_data = new List<TmpIntegrationMppCollectionTxn>();
        }
        public string mpp_code { get; set; }
        public string bmc_code { get; set; }
        public DateTime transaction_date { get; set; }
        public DateTime collection_date { get; set; }
        public string shift_code { get; set; }
        public int data_count { get; set; }

        public List<TmpIntegrationMppCollectionTxn> collection_data { get; set; }
    }

    public class TmpIntegrationMppCollectionTxn
    {
        public string farmer_code { get; set; }
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

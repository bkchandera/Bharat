using Dapper.Contrib.Extensions;

namespace Framework.Models
{   
    public class TaxMaster : BaseModel
    {
        public int tax_code { get; set; }
        public string tax_detail_code { get; set; }
        public decimal tax_amount { get; set; }
        [Computed]
        public int master_code { get; set; }
    }
}

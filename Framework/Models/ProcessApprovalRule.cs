

namespace Framework.Models
{ 
    public class ProcessApprovalRule : BaseModel
    {        
        public int process_code { get; set; }       
        public int approval_rule_code { get; set; }
        public decimal priority { get; set; }
        public string rule_name { get; set; }
        public string rule_table { get; set; }
        public string column_datatype { get; set; }
        public string propertyname { get; set; }
        public string propertyvalue { get; set; }
        public string rule_operator { get; set; }
    }

}

using Dapper.Contrib.Extensions;
using Framework.Library.Attribute;

namespace Framework.Models
{
    [Table("process_flow_detail")]
    [History("process_flow_detail_history")]
    public class ProcessFlowDetail : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int process_flow_detail_code { get; set; }
        public  int process_code { get; set; }
        public string user_code { get; set; }
        public  decimal levels { get; set; }
        public string rights { get; set; }
        public string for_user_code { get; set; }
        public string module_code { get; set; }
        public string module_name { get; set; }
        public  string approval_remarks { get; set; }
        public bool is_approve { get; set; } = false;
        public string process_status { get; set; } = "Pending";
    }
}

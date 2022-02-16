using Dapper.Contrib.Extensions;

namespace Framework.Models
{
    public class WorkFlowModel : BaseModel
    {
        public  decimal levels { get; set; } = 0;
        public  string status { get; set; } = "Approved";
        public  int process_code { get; set; }
        public  string approval_remarks { get; set; }  
        [Computed]
        public string process_flow_detail_code { get; set; }
    }
}

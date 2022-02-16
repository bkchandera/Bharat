using Dapper.Contrib.Extensions;

namespace Framework.Models
{
    [Table("process_flow")]
    public class ProcessFlow 
    {
        [Key]
        [ExplicitKey]
        public int process_flow_code { get; set; }
        public int process_code { get; set; }
        public string user_code { get; set; }
        public decimal levels { get; set; }
        public string rights { get; set; }
        public string for_user_code { get; set; }
        public int approval_rule_group_code { get; set; }

    }
}

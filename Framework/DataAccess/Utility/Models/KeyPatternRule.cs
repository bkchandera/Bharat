using Dapper.Contrib.Extensions;
using Framework.Models;

namespace Framework.DataAccess.Utility.Models
{
    [Table("key_pattern_rule")]
    public class KeyPatternRule : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int key_pattern_rule_code { get; set; }
        public string key_pattern_rule_name { get; set; }
        public int key_pattern_for_code { get; set; }
        public string prefix_field { get; set; }
        public int length { get; set; }
        public bool is_custom { get; set; }
        public string display_name { get; set; }
        public bool self_tr_code { get; set; }
        public bool child_tr_code { get; set; }
        public string table_name { get; set; }
      
        [Computed]
        public new string operation_type { get; set; }
    }
}

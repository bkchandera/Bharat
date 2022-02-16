using Dapper.Contrib.Extensions;
using Framework.Models;

namespace Framework.DataAccess.Utility.Models
{
    [Table("key_pattern_for")]
    public class KeyPatternFor : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int key_pattern_for_code { get; set; }        
        public string key_pattern_name { get; set; }
      
    }
}

using Dapper.Contrib.Extensions;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.DataAccess.Utility.Models
{
    [Table("company_key_pattern")]
    public class CompanyKeyPattern : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int company_key_pattern_code { get; set; }
        public int key_pattern_for_code { get; set; }
        public int key_pattern_rule_code { get; set; }
        public string company_code { get; set; }
    }
}

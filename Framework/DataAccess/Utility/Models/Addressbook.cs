using Dapper.Contrib.Extensions;
using Framework.Models;
using System;

namespace Framework.DataAccess.Utility.Models
{
    [Table("addressbook_portal")]
    public class Addressbook : BaseModel
    {
		[Key]
		[ExplicitKey]
		public int id { get; set; }
		public int? destinations { get; set; }
		public int? flag_entry { get; set; }
		public string organization_code { get; set; }
		public string organization_type { get; set; }
		public string source_org_type { get; set; }		
		public string table_name { get; set; }
		public int? to_child { get; set; }
		public int? to_parent { get; set; }
		public int? type { get; set; }		
		public string module_type { get; set; }
		public int priority { get; set; } = 1;
	}
}

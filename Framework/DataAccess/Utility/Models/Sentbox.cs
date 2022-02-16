using Dapper.Contrib.Extensions;
using Framework.Models;
using System;

namespace Framework.DataAccess.Utility.Models
{
	[Table("sentbox")]
	public class Sentbox : BaseModel
    {
		
			[ExplicitKey]
			public string uuid { get; set; }
			public string sync_status { get; set; } = "U";
			public string source_org_id { get; set; }
			public string message_type { get; set; } = "record";
			//from application table
			public string dest_org_id { get; set; }			
			public string dest_org_type { get; set; } = "bmc";
			public string error_log { get; set; }
			public string operation { get; set; }						
			public string originating_org_id { get; set; }
			public string originating_org_type { get; set; }			
			public int sequence_no { get; set; } = 0;
			//from application table
			public string device_id { get; set; }
			public string source_org_type { get; set; } = "company";
			public DateTime sync_timestamp { get; set; } = DateTime.Now;
			public string table_names { get; set; }			
			public int language_code { get; set; } = 0;
			public string version_no { get; set; }
			public DateTime posting_timestamp { get; set; } = DateTime.Now;
			public string json_text { get; set; }
		}	
}

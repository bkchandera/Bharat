using Dapper.Contrib.Extensions;
using Framework.Models;

namespace Framework.DataAccess.Utility.Models
{
    [Table("application_installation_detail")]
    class ApplicationInstallationDetail : BaseModel
    {
		[Key]
		[ExplicitKey]
		public int application_installation_detail_code { get; set; }
		public int? application_installation_code { get; set; }
		public string mobile_no { get; set; }
		public int? otp_code { get; set; }
		public string hash_key { get; set; }
		public bool? is_expired { get; set; }
		public string device_id { get; set; }
		public string device_type { get; set; }
		public string db_path { get; set; }
		public string use_for { get; set; }
		public string sync_key { get; set; }
		public bool? sync_active { get; set; }
		public int? installation_type { get; set; }
		public string version_no { get; set; }
	}
}

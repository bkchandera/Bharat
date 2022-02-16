using Dapper.Contrib.Extensions;
using Framework.Models;

namespace Framework.DataAccess.Utility.Models
{
    [Table("application_installation")]
    public class ApplicationInstallation : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int application_installation_code { get; set; }
        public string module_code { get; set; }
        public string module_name { get; set; }
        [Computed]
        public string device_id { get; set; }
    }
}

using Dapper.Contrib.Extensions;
using FluentValidation;
using Framework.Library.Validator;
using Framework.Models;

namespace DPUIntegration.Areas.Login.Models
{
    [Table("actions")]
    public partial class Actions : BaseModel
    {
        [Key]
        [ExplicitKey]
        public int action_code { get; set; }
        public string action_name { get; set; }
        public string portal_url { get; set; }
        public string service_url { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public bool is_free { get; set; }
        public int? parent_code { get; set; }

        /*
         *  1 --> Main Menu
         *  2 --> Sub Menu
         *  3 --> Third Level Menu
         **/
        public int menu_level { get; set; }

        public string api_key { get; set; }
        public int action_type { get; set; }
        public int action_for { get; set; }


    }

    public partial class ActionTreeView
    {
        public int action_code { get; set; }
        public string action_name { get; set; }
        public string portal_url { get; set; }
        public string service_url { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public bool is_free { get; set; }
        public int? parent_code { get; set; }
        public int menu_level { get; set; }
        public bool has_child { get; set; }
        public int? permission_code { get; set; }
        public string permission_name { get; set; }
        public ActionTreeView[] child_data { get; set; }
    }

    public class ActionsValidator : AbstractValidator<Actions>
    {
        public ActionsValidator()
        {
            RuleFor(d => d.action_name).Require();
            RuleFor(d => d.menu_level.ToString()).Require();
            RuleFor(d => d.action_for).Require();
        }
    }
}

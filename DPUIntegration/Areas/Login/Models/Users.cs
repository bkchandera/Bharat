using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Library.Validator;
using Framework.Models;
using Framework.Library.Attribute;
using Dapper.Contrib.Extensions;
using FluentValidation;

namespace DPUIntegration.Areas.Login.Models
{
    [Table("[users]")]
    public class Users : BaseModel
    {
        [ExplicitKey]
        public int user_code { get; set; }
        public string user_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile_no { get; set; }
        public string password { get; set; }
        public bool is_active { get; set; }

        [Computed]
        public string token { get; set; }

        [Computed]
        public bool change_flag { get; set; } = false;
        [Computed]
        public string old_password { get; set; }
        [Computed]
        public string tmp_password { get; set; }
    }


    public class TmpUser 
    {
        [ExplicitKey]
        public int user_code { get; set; }
        public string user_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile_no { get; set; }
        public bool is_active { get; set; }
        public string AuthToken { get; set; }

    }
    public class UsersValidator : AbstractValidator<Users>
    {
        public UsersValidator()
        {
            RuleFor(d => d.user_name).Require().Unique();
            RuleFor(d => d.email).Require();
            RuleFor(d => d.mobile_no).Require();
            RuleFor(d => d.password).Require();
            RuleFor(d => d.old_password).Equal(x => x.tmp_password).When(x => x.change_flag == true).WithMessage("old_password_not_match");
        }
    }
}

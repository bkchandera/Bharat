using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DPUIntegration.Areas.Login.Models;
using DPUIntegration.Helpers;
using Framework.BAL;
using Framework.CustomDataType;
using Framework.DataAccess.Redis;
using Framework.Extension;
using Framework.Library.Helper;
using Framework.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DPUIntegration.Areas.Login.BAL
{
    public class AuthBal : BaseBal<Users>
    {
        private List<ModelParameter> Data;
        private Users _user;
        private readonly AppSettings _appSettings;
        public AuthBal(Users user)
        {
            _user = new Users();
            _user = user;
        }
        public AuthBal() { }

        public AuthBal(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public CustomResult Login(ref string tokenString)
        {
            tokenString = DbHelper.UniqueKey();
            CacheRepository _cache = new CacheRepository();
            QueryParam QueryData = new QueryParam
            {
                Table = typeof(Users).GetTableName(),
                Fields = "users.user_name,users.user_code,users.name,users.email,users.mobile_no,users.is_active",
                Where = new List<ConditionParameter>{
                        Condition("user_name",_user.user_name),
                        Condition("password",_user.password),
                        Condition("users.is_active",1)
                }
            };
            TmpUser UsersModel = NewRepo.Find<TmpUser>(QueryData);
            if (UsersModel != null)
            {
                // authentication successful so generate jwt and refresh tokens
                var jwtToken = generateJwtToken(_user);
                UsersModel.AuthToken = jwtToken;

                TmpUserToken _TmpUserToken = new TmpUserToken
                {
                    username = UsersModel.user_name,
                    user_code = UsersModel.user_code.ToString(),
                    id = jwtToken,
                    exp = DateHelper.CurrentDate().AddHours(24),
                }; 
                _cache.SaveData(_TmpUserToken, jwtToken);

                UserToken ut = new UserToken
                {
                    user_code = UsersModel.user_code,
                    login_time = DateHelper.CurrentDate(),
                    token = jwtToken,
                    is_active = true
                };

                Data = new List<ModelParameter>()
                {
                    new ModelParameter{ ValidateModel=new UserTokenValidator(),SaveModel=ut}
                };

                this.SaveData(Data);
                return new CustomResult("success", UsersModel);
            }
            else
            {
                return new CustomResult("error", "wrong credentials");
            }
        }

        private string generateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.user_code.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void UpdateFreeAction()
        {
            CacheRepository _cache = new CacheRepository();
            FreeAction _freeAction = new FreeAction();
            QueryParam QueryData = new QueryParam()
            {
                Table = typeof(Actions).GetTableName(),
                Fields = "distinct service_url",
                Where = new List<ConditionParameter> {
                        Condition("is_free",true),
                        new ConditionParameter{direct_condition="service_url is not null",PropertyValue="#$#",PropertyName="service_url" }
                        //Condition("service_url",null," != "),
                    }
            };
            _freeAction.actions = NewRepo.FindAll<string>(QueryData).ToList();
            _freeAction.created_at = DateHelper.CurrentDate();
            _cache.SaveData(_freeAction, "FreeActions");
        }
    }
}


using System;
using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.DataAccess.Redis;
using Framework.Extension;
using Framework.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Framework.Library.Filter
{
    public class AuthenticateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext FilterContext)
        {
            StringValues Token, Type;
            bool IsToken = FilterContext.HttpContext.Request.Headers.TryGetValue("authorization", out Token);
            bool IsType = FilterContext.HttpContext.Request.Headers.TryGetValue("type", out Type);
            string CurrentUri = FilterContext.HttpContext.Request.Path;
            CacheRepository _cache = new CacheRepository();
            DBRepository repo = new DBRepository();
            if (FilterContext.HttpContext.Request.Method == "POST" && FilterContext.ActionArguments.ContainsKey("data"))
            {
                repo.Add(new QueryParam()
                {
                    DirectQuery = $"insert into request_log(host,request_url,payload,token) values('{FilterContext.HttpContext.Request.Host.Value}','{CurrentUri}','{JsonConvert.SerializeObject(FilterContext.ActionArguments["data"])}','{Token}')"
                });
            }
            else
            {
                repo.Add(new QueryParam()
                {
                    DirectQuery = $"insert into request_log(host,request_url,payload,token) values('{FilterContext.HttpContext.Request.Host.Value}','{CurrentUri}','{FilterContext.HttpContext.Request.QueryString}','{Token}')"
                });
            }

            #region old code
            /////if auth version change then we need to change here.
            //if (CurrentUri != "/auth/login" || CurrentUri.Contains("/api/v1.0/auth/login") || CurrentUri.Contains("/api/v1.0/auth/redisfreeaction"))
            //{
            //    CacheData _CacheData = new CacheData();
            //    FreeAction _freeAction = _CacheData.GetFreeAction();
            //    if (_freeAction != null)
            //    {
            //        if (_freeAction.actions.Contains(CurrentUri))
            //            return;
            //    }
            //}
            //else
            //{
            //    return;
            //} 
            #endregion

            string[] FreeAction = new string[3]
            {
                "/api/v1.0/auth/login",
                "/auth/login",
                "/api/v1.0/auth/redisfreeaction"
            };
            if (Array.IndexOf(FreeAction, CurrentUri) > -1)
            {
                return;
            }

            if (IsToken)
            {
                try
                {
                    TmpUserToken user = _cache.GetData<TmpUserToken>(Token);
                    if (user != null)
                    {
                        if (user.exp >= DateTime.Now)
                        {                            
                            UserData.token = Token;
                            UserData.user_code = user.user_code;
                            UserData.usertype = user.user_code;
                            UserData.company_code = user.company_code;
                            UserData.plant_code = user.plant_code;
                            UserData.mcc_code = user.mcc_code;
                            UserData.bmc_code = user.bmc_code;
                            UserData.mpp_code = user.mpp_code;
                            UserData.isAdmin = user.isAdmin;
                            UserData.tag = user.tag;
                            UserData.current_financial_year = user.current_financial_year;
                            user.exp = DateTime.Now.AddHours(1);
                            bool update_cached = _cache.UpdateData(Token, user);                            
                        }
                        else
                        {
                            bool clear_cached = _cache.DeleteData(Token);
                            FilterContext.Result = new CustomResult("error", "authentication_fail");
                        }
                    }
                    else
                    {
                        FilterContext.Result = new CustomResult("error", "authentication_fail");
                    }
                }
                catch
                {

                    FilterContext.Result = new CustomResult("error", "authentication_fail");
                }
            }
            else
            {

                FilterContext.Result = new CustomResult("error", "authentication_fail");
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }
}

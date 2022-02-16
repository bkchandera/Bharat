using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Extension
{
    public class DropdownEngine
    {
        public CustomResult Dropdown(object Data, string FileName)
        {
            DropdownData value = Data.ParseRequest<DropdownData>();
            JObject json = JObject.Parse(FileHelper.ReadFile(FileName));
            ListJsonDDL listObject = new ListJsonDDL();
            listObject = json.SelectToken(value.model).ToObject<ListJsonDDL>();
            if (value.fields != null)
            {
                DBRepository Repository = new DBRepository();
                
                if (listObject.where_data != null)
                {
                    string[] stringArray = listObject.where_data.Split(',');
                    if (listObject.where==null)
                        listObject.where = new List<ConditionParameter>();
                   
                    foreach (KeyValuePair<string, dynamic> entry in value.fields)
                    {
                        int pos = Array.IndexOf(stringArray.Select(x => x.Substring(x.IndexOf('.') + 1)).ToArray(), entry.Key);
                        if (pos > -1)
                        {
                            listObject.where.Add(new ConditionParameter { PropertyName = stringArray[pos], PropertyValue = entry.Value });
                        }

                    }
                    if (listObject.sp != null && listObject.sp != "" && stringArray.Contains("p_rls_user_code"))
                    {
                        listObject.where.Add(new ConditionParameter() { PropertyName = "p_rls_user_code", PropertyValue = UserData.user_code });
                    }
                }

                QueryParam Query;
                if (listObject.sp==null || listObject.sp == "")
                {
                    Query = new QueryParam
                    {
                        Distinct = listObject.distinct,
                        Table = listObject.model,
                        Fields = listObject.fields,
                        Join = listObject.join,
                        Where = listObject.where,
                        //OrderBy =  listObject.fields.Split(',')[1]
                        OrderBy = listObject.orderby == "" ? listObject.fields.Split(',')[1] : listObject.orderby
                    };
                    Query.DynemicParam = new Dictionary<string, dynamic>() {
                        {"rls_user_code", UserData.user_code }
                    };

                }
                else
                {                    
                    Query = new QueryParam()
                    {
                        Sp = listObject.sp,
                        Where = listObject.where,
                    };
                   
                }
                if (Query.Where != null)
                {
                    for (int i = 0; i < Query.Where.Count; i++)
                    {
                        if (Query.Where[i].direct_condition.Contains("###TAGDATA"))
                        {
                            Query.Where[i].direct_condition = Query.Where[i].direct_condition.Replace("###TAGDATA", UserData.tag);
                        }

                    }
                }

                List<IDictionary<string, object>> ObjectResult = Repository.FindAll(Query).Select(x => (IDictionary<string, object>)x).ToList();

                List<ReturnValue> FinalResult = ObjectResult.Select(a => new ReturnValue { id = a.Values.FirstOrDefault().ToString(), value = a.Values.LastOrDefault().ToString() }).ToList();

                return new CustomResult("success", FinalResult);
            }
            return new CustomResult("error");
        }

        //for rls method
        public CustomResult Dropdown(object Data, string FileName,string RlsColumnName,List<string> RlsColumnValue=null)
        {
            DropdownData value = Data.ParseRequest<DropdownData>();
            JObject json = JObject.Parse(FileHelper.ReadFile(FileName));
            ListJsonDDL listObject = new ListJsonDDL();
            listObject = json.SelectToken(value.model).ToObject<ListJsonDDL>();
            if (value.fields != null)
            {
                DBRepository Repository = new DBRepository();
                if (listObject.where_data != null)
                {
                    if (listObject.where == null)
                        listObject.where = new List<ConditionParameter>();
                    string[] stringArray = listObject.where_data.Split(',');
                    foreach (KeyValuePair<string, dynamic> entry in value.fields)
                    {
                        int pos = Array.IndexOf(stringArray.Select(x => x.Substring(x.IndexOf('.') + 1)).ToArray(), entry.Key);
                        if (pos > -1)
                        {
                            listObject.where.Add(new ConditionParameter { PropertyName = stringArray[pos], PropertyValue = entry.Value });
                        }

                    }

                    if (RlsColumnValue!=null && RlsColumnValue.FirstOrDefault()!=null)
                    {
                        listObject.where.Add(new ConditionParameter { PropertyName = RlsColumnName, PropertyValue = RlsColumnValue.ToArray() ,Operator="IN"});
                    }
                }
                List<IDictionary<string, object>> ObjectResult = Repository.FindAll(new QueryParam
                {                   
                    Table = listObject.model,
                    Fields = listObject.fields,
                    Join = listObject.join,
                    Where = listObject.where,
                    //OrderBy =  listObject.fields.Split(',')[1]
                    OrderBy = listObject.orderby == "" ? listObject.fields.Split(',')[1] : listObject.orderby
                }).Select(x => (IDictionary<string, object>)x).ToList();

                List<ReturnValue> FinalResult = ObjectResult.Select(a => new ReturnValue { id = a.Values.FirstOrDefault().ToString(), value = a.Values.LastOrDefault().ToString() }).ToList();

                return new CustomResult("success", FinalResult);
            }
            return new CustomResult("error");
        }
    }
}

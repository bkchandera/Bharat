using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Extension
{
    public class ListEngine
    {
        /*
         * Data : Search Param
         * File name : File name without main Directory eg. "\\Areas\\General\\Config\\List.json"
         * Token : name of token through which we have to find data from json file
         */
        public CustomResult List(object Data, string FileName, string Token)
        {
            try
            {

                ListData listData = new ListData();

                //convert data to search param object
                SearchParam Search = Data.ParseRequest<SearchParam>();

                if (Search.model.NotEmpty())
                {
                    Token = Search.model;
                }

                // read list.json file
                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FileName(FileName)));
                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();

                //find all required parameter which should be in search filter
                string[] require = (json.SelectToken($"{Token}.require") ?? "").ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (Query.Sp == null || Query.Sp.Trim() == "")
                {
                    Query.DynemicParam = new Dictionary<string, dynamic>() {
                        {"rls_user_code", UserData.user_code }
                    };                    
                }
                else
                {
                    if (require.Contains("p_rls_user_code"))
                    {
                        if (Search.Param == null)
                            Search.Param = new List<ConditionParameter>();
                        Search.Param.Add(new ConditionParameter() { PropertyName = "p_rls_user_code", PropertyValue = UserData.user_code });
                    }
                    if (require.Contains("p_user_tag"))
                    {
                        if (Search.Param == null)
                            Search.Param = new List<ConditionParameter>();
                        Search.Param.Add(new ConditionParameter() { PropertyName = "p_user_tag", PropertyValue = UserData.tag });
                    }
                }
                foreach (string s in require)
                {
                    if (Search.Param == null || !Search.Param.Any(x => x.PropertyName.ReplaceSubstring() == s.ReplaceSubstring()))
                    {
                        return new CustomResult("validation_error", s.ReplaceSubstring().GetErrors("require"));
                    }

                }
                Query.Where = Query.Where.Append(Search.Param);    
                //old rls condition
                //if (json.SelectToken($"{Token}.rls_column") != null)
                //{
                //    string[] RlsColumnList = json.SelectToken($"{Token}.rls_column").ToString().Split(',');
                //    foreach(string RlsColumn in RlsColumnList)
                //    {                        
                //        if (RlsColumn != "" )
                //        {
                //                List<string> RlsColumnValue;
                //                if (Hierarchy.TryGetValue(RlsColumn.Substring(RlsColumn.IndexOf('.') + 1), out RlsColumnValue))
                //                {
                //                    if (RlsColumnValue != null && RlsColumnValue.FirstOrDefault() != null)
                //                    {
                //                        if (Query.Sp==null || Query.Sp.Trim() == "")
                //                        {
                //                            if (!Query.Where.Any(x => x.PropertyName == RlsColumn))
                //                            {
                //                                ConditionParameter RlsCondition = new ConditionParameter { PropertyName = RlsColumn, PropertyValue = RlsColumnValue.ToArray(), Operator = "IN" };
                //                                Query.Where.Add(RlsCondition);
                //                            }
                //                        }
                //                        else
                //                        {
                //                            Query.Where.Where(x => x.PropertyName == RlsColumn && x.PropertyValue == "0").ToList().ForEach(x =>  x.PropertyValue =string.Join(',',RlsColumnValue));
                //                        }
                                        
                //                    }
                //                }                                                     
                //        }
                //    }                                      
                //}

               
                
                Query.Offset = Search.Offset;
                Query.Limit = Search.Limit;
                if (Query.child != null)
                {
                    int i = 0;
                    if (Search.type == null)
                        return new CustomResult("validation_error", "type".GetErrors("require"));
                    foreach (ChildData Tmp in Query.child)
                    {
                        if (Tmp.type == Search.type)
                        {
                            Query.Fields = (Tmp.fields + "," + Query.Fields).Trim(',');
                            Query.Join = Query.Join.Append(Tmp.Join);
                            Query.Where = Query.Where.Append(Tmp.Where);
                            Query.GroupBy = (Query.GroupBy + "," + Tmp.groupby).Trim(',');
                            i++;
                        }
                    }
                    if (i == 0)
                        return new CustomResult("validation_error", "type".GetErrors("mismatch"));
                }
                //if (UserData.isAdmin)
                //{
                //    Query.Join.Where(x => x.type.Contains("###ADMINUSER")).ToList().ForEach(x => x.type="LEFT");
                //}
                //else
                //{
                //    Query.Join.Where(x => x.type.Contains("###ADMINUSER")).ToList().ForEach(x => x.type = "INNER");
                //}
                // foreach(JoinParameter Jparam in Query.Join)
                if (Query.Join != null)
                {
                    for (int i = 0; i < Query.Join.Count; i++)
                    {
                        if (UserData.isAdmin && Query.Join[i].table == "process_flow_detail")
                        {
                            Query.Join[i].type = "LEFT";
                        }
                        else if (!UserData.isAdmin && Query.Join[i].table == "process_flow_detail")
                        {
                            Query.Join[i].type = "INNER";
                        }


                        if (Query.Join[i].condition.Contains("###TAGDATA"))
                            Query.Join[i].condition = Query.Join[i].condition.Replace("###TAGDATA", UserData.tag);
                    }
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


                DBRepository NewRepo = new DBRepository();
                List<object> FinalList = NewRepo.FindAll<object>(Query).ToList();
                listData.data_list = FinalList;
                //old header logic
                //listData.header = Query.Fields.Split(',').Where(e => !e.EndsWith(')')).ToArray().ReplaceSubstring(" as ").ReplaceSubstring();
                //find is_header is true or false
                bool is_header = (json.SelectToken($"{Token}.is_header") == null || json.SelectToken($"{Token}.is_header").ToString() == "true" )? true : false;

                if (is_header)
                {
                    if (FinalList.Count > 0)
                    {
                        IDictionary<string, object> TmpHeader = FinalList.FirstOrDefault() as IDictionary<string, object>;
                        listData.header = TmpHeader.Select(x => x.Key).ToArray();
                    }
                    else
                    {
                        listData.header = new string[] { "data_not_available" };
                    }
                }
                return new CustomResult("success", listData);
            }
            catch (Exception E)
            {
                return new CustomResult("error", E.Message);
            }
        }

        public CustomResult List(object Data, string FileName, string Token, Dictionary<string, string> FieldValue = null, List<ConditionParameter> WhereValue = null, Dictionary<string, string> JoinValue = null, Dictionary<string, dynamic> DynemicParameter = null)
        {
            try
            {
                Dictionary<string, List<string>> Hierarchy = new Dictionary<string, List<string>>
                {
                    {"company_code",UserData.company_code },
                    {"plant_code",UserData.plant_code },
                    {"mcc_code",UserData.mcc_code },
                    {"bmc_code",UserData.bmc_code },
                    {"mpp_code",UserData.mpp_code }
                };

                ListData listData = new ListData();

                //convert data to search param object
                SearchParam Search = Data.ParseRequest<SearchParam>();


                if (Search.model.NotEmpty())
                {
                    Token = Search.model;
                }

                // read list.json file
                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FileName(FileName)));
                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();

                //find all required parameter which should be in search filter
                string[] require = (json.SelectToken($"{Token}.require") ?? "").ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (Query.Sp == null || Query.Sp.Trim() == "")
                {
                    if (DynemicParameter == null)
                        DynemicParameter = new Dictionary<string, dynamic>
                        {
                            {"rls_user_code",UserData.user_code }
                        };
                    else
                        DynemicParameter.Add("rls_user_code", UserData.user_code);
                   
                    
                }
                else
                {
                    if (require.Contains("p_rls_user_code"))
                    {
                        if (Search.Param == null)
                            Search.Param = new List<ConditionParameter>();
                        Search.Param.Add(new ConditionParameter() { PropertyName = "p_rls_user_code", PropertyValue = UserData.user_code });
                    }
                    if (require.Contains("p_user_tag"))
                    {
                        if (Search.Param == null)
                            Search.Param = new List<ConditionParameter>();
                        Search.Param.Add(new ConditionParameter() { PropertyName = "p_user_tag", PropertyValue = UserData.tag });
                    }
                }
                foreach (string s in require)
                {
                    if (Search.Param == null || !Search.Param.Any(x => x.PropertyName.ReplaceSubstring() == s.ReplaceSubstring()))
                    {
                        return new CustomResult("validation_error", s.ReplaceSubstring().GetErrors("require"));
                    }

                }

                Query.Where = Query.Where.Append(Search.Param);
               
                Query.Offset = Search.Offset;
                Query.Limit = Search.Limit;

                if (Query.child != null)
                {
                    int i = 0;
                    if (Search.type == null)
                        return new CustomResult("validation_error", "type".GetErrors("require"));
                    foreach (ChildData Tmp in Query.child)
                    {
                        if (Tmp.type == Search.type)
                        {
                            Query.Fields = (Tmp.fields + "," + Query.Fields).Trim(',');
                            Query.Join = Query.Join.Append(Tmp.Join);
                            Query.Where = Query.Where.Append(Tmp.Where);
                            Query.GroupBy = (Query.GroupBy + "," + Tmp.groupby).Trim(',');
                            i++;
                        }
                    }
                    if (i == 0)
                        return new CustomResult("validation_error", "type".GetErrors("mismatch"));
                }

                if (FieldValue != null)
                {
                    foreach (KeyValuePair<string, string> Fields in FieldValue)
                    {
                        Query.Fields = Query.Fields.Replace(Fields.Key, Fields.Value);
                    }
                }
                foreach (KeyValuePair<string, string> Fields in JoinValue.NotEmpty())
                {
                    Query.Join.ForEach(s => s.condition = s.condition.Replace(Fields.Key, Fields.Value));
                }
                foreach (ConditionParameter NewValue in WhereValue.NotEmpty())
                {
                    Query.Where.Where(x => x.PropertyName == NewValue.PropertyName).ToList().ForEach(x => { x.PropertyValue = NewValue.PropertyValue; x.Operator = NewValue.Operator; });
                }                
                Query.DynemicParam = DynemicParameter;

                if (Query.Join != null)
                {
                    for (int i = 0; i < Query.Join.Count; i++)
                    {
                        if (UserData.isAdmin && Query.Join[i].table == "process_flow_detail")
                        {
                            Query.Join[i].type = "LEFT";
                        }
                        else if (!UserData.isAdmin && Query.Join[i].table == "process_flow_detail")
                        {
                            Query.Join[i].type = "INNER";
                        }


                        if (Query.Join[i].condition.Contains("###TAGDATA"))
                            Query.Join[i].condition = Query.Join[i].condition.Replace("###TAGDATA", UserData.tag);
                    }
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


                DBRepository NewRepo = new DBRepository();
                //listData.data_list = NewRepo.FindAll<object>(Query).ToList();
                //listData.header = Query.Fields.Split(',').Where(e => !e.EndsWith(')')).ToArray().ReplaceSubstring(" as ").ReplaceSubstring();

                List<object> FinalList = NewRepo.FindAll<object>(Query).ToList();
                listData.data_list = FinalList;

                //find is_header is true or false
                bool is_header = ((json.SelectToken($"{Token}.is_header")) == null ? true : Convert.ToBoolean((json.SelectToken($"{Token}.is_header").ToString())));

                if (is_header)
                {
                    if (FinalList.Count > 0)
                    {
                        IDictionary<string, object> TmpHeader = FinalList.FirstOrDefault() as IDictionary<string, object>;
                        listData.header = TmpHeader.Select(x => x.Key).ToArray();
                    }
                    else
                    {
                        listData.header = new string[] { "data_not_available" };
                    }
                }
                return new CustomResult("success", listData);
            }
            catch (Exception E)
            {
                return new CustomResult("error", E.Message);
            }
        }


        public List<T> List<T>(object Data, string FileName, string Token, Dictionary<string, string> FieldValue = null, List<ConditionParameter> WhereValue = null, Dictionary<string, string> JoinValue = null, Dictionary<string, dynamic> DynemicParameter = null) where T : class
        {
            try
            {

                SearchParam Search = Data.ParseRequest<SearchParam>();

                // read list.json file
                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FileName(FileName)));
                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();

                Query.Where = Query.Where.Append(Search.Param);

                string[] require = (json.SelectToken($"{Token}.require") ?? "").ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                //if (Query.Sp == null || Query.Sp.Trim() == "")
                //{
                //    if (DynemicParameter == null)
                //        DynemicParameter = new Dictionary<string, dynamic>
                //        {
                //            {"rls_user_code",UserData.user_code }
                //        };
                //    else
                //        DynemicParameter.Add("rls_user_code", UserData.user_code);
                //}
                //else
                //{
                //    if (require.Contains("p_rls_user_code"))
                //    {
                //        if (Search.Param == null)
                //            Search.Param = new List<ConditionParameter>();
                //        Search.Param.Add(new ConditionParameter() { PropertyName = "p_rls_user_code", PropertyValue = UserData.user_code });
                //    }
                //}

                Query.Offset = Search.Offset;
                Query.Limit = Search.Limit;

                if (Query.child != null)
                {
                    int i = 0;
                    if (Search.type == null)
                        return null;
                    foreach (ChildData Tmp in Query.child)
                    {
                        if (Tmp.type == Search.type)
                        {
                            Query.Fields = (Tmp.fields + "," + Query.Fields).Trim(',');
                            Query.Join = Query.Join.Append(Tmp.Join);
                            Query.Where = Query.Where.Append(Tmp.Where);
                            Query.GroupBy = (Query.GroupBy + "," + Tmp.groupby).Trim(',');
                            i++;
                        }
                    }
                    if (i == 0)
                        return null;
                }

                if (FieldValue != null)
                {
                    foreach (KeyValuePair<string, string> Fields in FieldValue)
                    {
                        Query.Fields = Query.Fields.Replace(Fields.Key, Fields.Value);
                    }
                }
                foreach (KeyValuePair<string, string> Fields in JoinValue.NotEmpty())
                {
                    Query.Join.ForEach(s => s.condition = s.condition.Replace(Fields.Key, Fields.Value));
                }
                foreach (ConditionParameter NewValue in WhereValue.NotEmpty())
                {
                    Query.Where.Where(x => x.PropertyName == NewValue.PropertyName).ToList().ForEach(x => { x.PropertyValue = NewValue.PropertyValue; x.Operator = NewValue.Operator; });
                }
                Query.DynemicParam = DynemicParameter;

                if (Query.Join != null)
                {
                    for (int i = 0; i < Query.Join.Count; i++)
                    {
                        if (UserData.isAdmin && Query.Join[i].table == "process_flow_detail")
                        {
                            Query.Join[i].type = "LEFT";
                        }
                        else if (!UserData.isAdmin && Query.Join[i].table == "process_flow_detail")
                        {
                            Query.Join[i].type = "INNER";
                        }


                        if (Query.Join[i].condition.Contains("###TAGDATA"))
                            Query.Join[i].condition = Query.Join[i].condition.Replace("###TAGDATA", UserData.tag);
                    }
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

                DBRepository NewRepo = new DBRepository();
                return NewRepo.FindAll<T>(Query).ToList();
            }
            catch 
            {
                return null;
            }
        }
    }
}

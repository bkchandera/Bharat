using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Extension
{
    public class ViewEngine
    {
        public CustomResult View(string FileName, string Token)
        {
            try
            {

                // read list.json file
                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FileName(FileName)));
                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();
                DBRepository NewRepo = new DBRepository();
                return new CustomResult("success", NewRepo.Find<object>(Query));
            }
            catch (Exception E)
            {
                return new CustomResult("error", E.Message);
            }
        }

        public CustomResult View(object Data, string FileName, string Token)
        {
            try
            {

                // read list.json file
                JObject json = JObject.Parse(FileHelper.ReadFile(FileHelper.FileName(FileName)));

                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();

                DropdownData value = Data.ParseRequest<DropdownData>();

                //find all required parameter which should be in search filter
                string[] require = (json.SelectToken($"{Token}.require") ?? "").ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);


                foreach (string s in require)
                {
                    if (value.fields==null || !value.fields.Any(x => x.Key.ReplaceSubstring() == s.ReplaceSubstring()))
                    {
                        return new CustomResult("validation_error", s.ReplaceSubstring().GetErrors("Require"));
                    }
                }
                if (value.fields != null)
                {
                    if (Query.Where == null)
                        Query.Where = new List<ConditionParameter>();
                    foreach (KeyValuePair<string, dynamic> entry in value.fields)
                    {
                        Query.Where.Add(new ConditionParameter { PropertyName = entry.Key, PropertyValue = entry.Value });
                    }

                }
                
                if (Query.child != null)
                {
                    int i = 0;
                    if (value.model == null)
                        return new CustomResult("validation_error", "type".GetErrors("require"));
                    foreach (ChildData Tmp in Query.child)
                    {
                        if (Tmp.type == value.model)
                        {
                            Query.Fields = (Query.Fields + "," + Tmp.fields).Trim(',');
                            Query.Join = Query.Join.Append(Tmp.Join);
                            Query.Where = Query.Where.Append(Tmp.Where);                          
                            i++;
                        }
                    }
                    if (i == 0)
                        return new CustomResult("validation_error", "type".GetErrors("mismatch"));
                }

                DBRepository NewRepo = new DBRepository();                
                return new CustomResult("success", NewRepo.Find<object>(Query));
            }
            catch (Exception E)
            {
                return new CustomResult("error", E.Message);
            }
        }

        public CustomResult View(object Data, string FileName, string Token,bool flag,string FullName=null)
        {
            try
            {



                SearchParam Search = Data.ParseRequest<SearchParam>();
                JObject json;
                // read list.json file
                if (FullName==null)
                    json = JObject.Parse(FileHelper.ReadFile(FileHelper.FileName(FileName)));
                else
                    json = JObject.Parse(FileHelper.ReadFile(FileHelper.ProjectPath()+ FullName));
                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();

                //find all required parameter which should be in search filter
                string[] require = (json.SelectToken($"{Token}.require") ?? "").ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in require)
                {
                    if (Search.Param == null || !Search.Param.Any(x => x.PropertyName.ReplaceSubstring() == s.ReplaceSubstring()))
                    {
                        return new CustomResult("validation_error", s.ReplaceSubstring().GetErrors("require"));
                    }

                }
                if (Query.Where != null)
                    Query.Where.AddRange(Search.Param);
                else
                    Query.Where = Search.Param;
                
                DBRepository NewRepo = new DBRepository();
                return new CustomResult("success", NewRepo.Find<object>(Query));
            }
            catch (Exception E)
            {
                return new CustomResult("error", E.Message);
            }
        }

        public T View<T>(string FileName, string Token,JObject Data=null,string FullFileName=null) where T : class
        {
            try
            {
                DBRepository NewRepo = new DBRepository();               
                // read list.json file
                string tmp;
                if (FullFileName != null)
                {
                    tmp = FullFileName;
                }
                else
                {
                    tmp = FileHelper.FileName(FileName);
                }               
                JObject json = JObject.Parse(FileHelper.ReadFile(tmp));
                QueryParam Query = json.SelectToken(Token).ToObject<QueryParam>();
                if(Data!=null)
                    Query.Where.Where(x => x.PropertyValue == "?").ToList().ForEach(x => x.PropertyValue = Data.GetValue(x.PropertyName).ToString());                                
                
                return NewRepo.Find<T>(Query);
                
            }
            catch (Exception E)
            {
                //LoggerFactory loggerFactory = new LoggerFactory();
                //loggerFactory.AddFile($"{FileHelper.ProjectPath()}//FileServer//mylog.txt");
                //Microsoft.Extensions.Logging.ILogger logger1 = loggerFactory.CreateLogger("test");
                //logger1.LogInformation(E.Message);
                DBRepository NewRepo = new DBRepository();  
                QueryParam Query = new QueryParam
                {
                    DirectQuery= $"insert into error_log(error_log) values('{E.Message.Replace('\'',' ')}')",                    
                };
                NewRepo = new DBRepository();
                NewRepo.Add(Query);
                return null;

            }
        }
    }
}

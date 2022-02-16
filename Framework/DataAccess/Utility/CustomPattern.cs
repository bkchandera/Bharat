using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.DataAccess.Utility.Models;
using Framework.Extension;
using Framework.Library.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Framework.DataAccess.Utility
{
    public class CustomPattern
    {
        public string GenerateCode(string Type, string code_value = "")
        {
            string[] tmp = Generate(Type, code_value);            
            return tmp[0] + (Convert.ToInt32(tmp[1]) + 1).ToString().PadLeft(Convert.ToInt32(tmp[2]), '0');
        }
        public string[] GenerateCode(string code_value = "")
        {
            return Generate("member", code_value);
        }
        
        public KeyPatternRule PatternCode(string Type,string CompanyCode)
        {
            JObject JObjectObj = new JObject
            {
                { "key_pattern_name", Type },
                { "company_code", CompanyCode }
            };
            ViewEngine viewEngine = new ViewEngine();
            return viewEngine.View<KeyPatternRule>(null, "KeyPatternRuleOperation", JObjectObj, FileHelper.FrameworkPath("DataAccess\\Utility\\Config\\Custom.json"));
        }
        public bool IsGenerate(string Type,string CompanyCode,string check_type="self")
        {
            JObject JObjectObj = new JObject
            {
                { "key_pattern_name", Type },
                { "company_code", CompanyCode }
            };
            ViewEngine viewEngine = new ViewEngine();
            KeyPatternRule KeyPatternRuleModel = viewEngine.View<KeyPatternRule>(null, "IsGenerate", JObjectObj, FileHelper.FrameworkPath("DataAccess\\Utility\\Config\\Custom.json"));
            if (check_type == "self")
                return KeyPatternRuleModel.self_tr_code;
            else
                return KeyPatternRuleModel.child_tr_code;
        }
        private string[] Generate(string Type, string code_value = "")
        {
            JObject JObjectObj = new JObject
            {
                { "key_pattern_name", Type },
                { "company_code", "01" }
            };
            ViewEngine viewEngine = new ViewEngine();
            KeyPatternRule KeyPatternRuleModel = viewEngine.View<KeyPatternRule>(null, "KeyPatternRule", JObjectObj, FileHelper.FrameworkPath("DataAccess\\Utility\\Config\\Custom.json"));
            DBRepository repository = new DBRepository();
            QueryParam Query = new QueryParam();
            string FieldName = Type + "_tr_code";
            if (Type == "member" && KeyPatternRuleModel.prefix_field == "hamlet_code")
            {
                return StaticMemberCode(code_value, KeyPatternRuleModel.length);
            }
            else if (KeyPatternRuleModel.is_custom)
            {
                Query.DirectQuery = $"SELECT isnull(max(cast({KeyPatternRuleModel.table_name}.{FieldName} as int)),0) FROM {KeyPatternRuleModel.table_name} ";
                //return GeneratePK.getKey(KeyPatternRuleModel.field_name, KeyPatternRuleModel.table_name, KeyPatternRuleModel.length, code_value);
            }
            else
            {
                Query.DirectQuery = $"SELECT isnull(max(cast(SUBSTRING({KeyPatternRuleModel.table_name}.{FieldName},{ code_value.Length + 1},LEN({KeyPatternRuleModel.table_name}.{ FieldName})) as int)),0) FROM {KeyPatternRuleModel.table_name} where SUBSTRING({KeyPatternRuleModel.table_name}.{FieldName},1,{code_value.Length})='{code_value}'";
            }
            //string[] ReturnValue = new string[3];
            //ReturnValue[0] = code_value;
            //ReturnValue[1] = repository.Find<string>(Query);
            //ReturnValue[2] = KeyPatternRuleModel.length.ToString();
            //return ReturnValue;
            return new string[] { code_value, repository.Find<string>(Query), KeyPatternRuleModel.length.ToString() };
        }
        private string[] StaticMemberCode(string code_value, int length)
        {
            DBRepository repository = new DBRepository();
            QueryParam HamletQuery = new QueryParam
            {
                Fields = "concat(state_code,hamlet_code)",
                Table = "business_address_detail",
                Where = new List<ConditionParameter>
                        {
                            new ConditionParameter{PropertyName="module_code",PropertyValue=code_value },
                             new ConditionParameter{PropertyName="module_name",PropertyValue="mpp" },
                            new ConditionParameter{PropertyName="is_default",PropertyValue="1"},
                        }
            };
            string hamlet_code = repository.Find<string>(HamletQuery);

            QueryParam Query = new QueryParam
            {
                DirectQuery = $@"select isnull(cast(max(SUBSTRING(member_tr_code,{hamlet_code.Length+1},LEN(member_tr_code))) as varchar(15)),0)"+
                                        $"FROM  member_sahayak_hierarchy where SUBSTRING(member_tr_code,1,{hamlet_code.Length})='{hamlet_code}'",

            };
            //string[] ReturnValue = new string[3];
            //ReturnValue[0] = hamlet_code;
            //ReturnValue[1] = repository.Find<string>(Query);
            //ReturnValue[2] = length.ToString();
            return new string[] { hamlet_code, repository.Find<string>(Query), length.ToString() };
            //ReturnValue;
        }
    }
}

using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Framework.BAL
{
    public class WorkFlow
    {

        private string _code;
        private int _process;
        private List<BaseModel> _SaveModelList;
        private QueryParam _query;
        private DBRepository _repo;

        public WorkFlow(int process, List<BaseModel> SaveModelList, string code)
        {
            _process = process;
            _SaveModelList = SaveModelList;
            _code = code;
            _repo = new DBRepository();
        }
        public WorkFlow()
        {

        }
        public List<ModelParameter> SaveWorkFlow(List<BaseModel> SaveModelList, int processcode, string code, ref decimal level)
        {
            _repo = new DBRepository();
            _process = processcode;
            _code = code;
            _SaveModelList = SaveModelList;
            List<ProcessFlow> FLowList;
            FLowList = CompareRule();
            if (FLowList != null && FLowList.Count() > 0)
            {
                List<ModelParameter> Data = new List<ModelParameter>();
                List<ProcessFlowDetail> OldData = _repo.FindAll<ProcessFlowDetail>(new List<ConditionParameter> {
                    new ConditionParameter{PropertyName="process_code",PropertyValue=_process},
                    new ConditionParameter{PropertyName="module_code",PropertyValue=_code},
//                     new ConditionParameter{PropertyName="levels",PropertyValue=0,Operator="!="},
                });
                if(OldData!=null && OldData.Count() > 0)
                {                    
                    foreach (ProcessFlowDetail DetailModel in OldData)
                    {
                        DetailModel.model_operation = "delete";
                        Data.Add(new ModelParameter { ValidateModel = null, SaveModel = DetailModel });
                    }
                }
                
                int i = 0;
                foreach (ProcessFlow FlowModel in FLowList)
                {
                    bool is_approve = (i == 0) ? true : false;
                    if (i == 1)
                    {
                        level = FlowModel.levels;
                    }
                    i++;
                    Data.Add(new ModelParameter
                    {
                        ValidateModel = null,
                        SaveModel = new ProcessFlowDetail
                        {
                            process_code = _process,
                            user_code = FlowModel.user_code,
                            for_user_code = FlowModel.for_user_code,
                            levels = FlowModel.levels,
                            rights = FlowModel.rights,
                            module_code = _code,
                            module_name = _process.ToString(),
                            is_approve= is_approve
                        }
                    });
                }
                return Data;
            }
            return null;


        }
        private List<ProcessFlow> CompareRule()
        {
            int? MasterCode;
            MasterCode = GetMasterCode(UserData.user_code, true);
            if (MasterCode != null && CheckRules(MasterCode))
            {
                return UserLevelList(MasterCode);
            }
            MasterCode = GetMasterCode("0", true);
            if (MasterCode != null && CheckRules(MasterCode))
            {
                return UserLevelList(MasterCode);
            }
            MasterCode = GetMasterCode(UserData.user_code);
            if (MasterCode != null)
            {
                return UserLevelList(MasterCode);
            }
            MasterCode = GetMasterCode("0");
            return UserLevelList(MasterCode);

        }

        private List<ProcessFlow> UserLevelList(int? code)
        {
            _query = new QueryParam
            {
                Table = "process_flow",
                Fields = "user_code,levels,rights,for_user_code",
                Where = new List<ConditionParameter>
                {
                    new ConditionParameter{PropertyName="process_flow.process_flow_master_code",PropertyValue=code},
                    new ConditionParameter{PropertyName="process_flow.for_user_code",PropertyValue=UserData.user_code},
                },
                OrderBy = "levels"
            };
            return _repo.FindAll<ProcessFlow>(_query).ToList();
        }
        private bool CheckRules(int? master_code)
        {

            _query = new QueryParam
            {
                Table = "process_flow_rule",
                Fields = "process_code,priority,process_flow_rule.approval_rule_code,rule_table,propertyname,rule_operator,process_flow_rule.propertyvalue,process_approval_rule.column_datatype",
                Join = new List<JoinParameter>
                {
                    new JoinParameter{table="process_approval_rule",condition="process_approval_rule.approval_rule_code=process_flow_rule.approval_rule_code"},
                },
                Where = new List<ConditionParameter>
                {
                    new ConditionParameter{PropertyName="process_flow_rule.process_flow_master_code",PropertyValue=master_code}
                },
                OrderBy = "priority"
            };
            List<ProcessApprovalRule> RuleList = _repo.FindAll<ProcessApprovalRule>(_query).ToList();
            decimal[] GroupList = RuleList.Select(x => x.priority).Distinct().ToArray();
            string condition;

            if (GroupList.Count() > 0)
            {
                foreach (decimal code in GroupList.NotEmpty())
                {
                    bool flag = false;
                    List<ProcessApprovalRule> grouplist = RuleList.Where(x => x.priority == code).ToList();
                    foreach (ProcessApprovalRule rule in grouplist)
                    {
                        if (rule.column_datatype.ToLower() == "string")
                            condition = $"{rule.column_datatype}(it[\"{rule.propertyname}\"]){rule.rule_operator}\"{rule.propertyvalue}\"";
                        else
                            condition = $"{rule.column_datatype}(it[\"{rule.propertyname}\"]){rule.rule_operator}{rule.propertyvalue}";
                        //int cnt = _SaveModelList.Where(x => x.GetType().GetTableName().Trim() == rule.rule_table.Trim()).Select(x=>JObject.FromObject(x)).ToList<JObject>().AsQueryable().Where(condition).Count();
                        List<JObject> comparelist = _SaveModelList.Where(x =>x.model_operation!="delete" && x.GetType().GetTableName().Trim() == rule.rule_table.Trim()).Select(x => JObject.FromObject(x)).ToList<JObject>();
                        int cnt = comparelist.AsQueryable().Where(condition).Count();
                        if (cnt <= 0)
                        {
                            break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        private int? GetMasterCode(string UserCode, bool flag = false)
        {
            int rule = 0;
            _query = new QueryParam
            {
                Table = "process_flow",
                Fields = "process_flow.process_flow_master_code",
                Where = new List<ConditionParameter>
                    {
                        new ConditionParameter{PropertyName="process_code",PropertyValue=_process},
                        new ConditionParameter{PropertyName="for_user_code",PropertyValue=UserCode},
                    },
                OrderBy="levels"
            };
            if (flag)
            {
                _query.Join = new List<JoinParameter>
                {
                    new JoinParameter{table="process_flow_rule",condition="process_flow.process_flow_master_code=process_flow_rule.process_flow_master_code"}
                };
                rule = 1;
            }
            _query.Where.Add(new ConditionParameter { PropertyName = "process_flow.approval_rule", PropertyValue = rule });
            return _repo.Find<int?>(_query);
        }
    }
}

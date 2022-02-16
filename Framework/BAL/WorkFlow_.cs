using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.DataAccess.Utility.Models;
using Framework.Library.Helper;
using Framework.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Framework.BAL
{
    public class WorkFlow_
    {
        private string _code;
        private int _process;
        private List<BaseModel> _SaveModelList;
        private QueryParam _query;
        private DBRepository _repo;
        private List<ProcessFlow> FLowList;
        public WorkFlow_(int process, List<BaseModel> SaveModelList,string code)
        {
            _process = process;
            _SaveModelList = SaveModelList;
            _code = code;
            _repo = new DBRepository();
        }
        public List<ModelParameter> SaveWorkFlow()
        {
            CompareRule();
            if (FLowList != null)
            {
                List<ModelParameter> Data = new List<ModelParameter>();
                List<ProcessFlowDetail> OldData = _repo.FindAll<ProcessFlowDetail>(new List<ConditionParameter> {
                    new ConditionParameter{PropertyName="process_code",PropertyValue=_process},
                    new ConditionParameter{PropertyName="module_code",PropertyValue=_code},
                    new ConditionParameter{PropertyName="levels",PropertyValue=0,Operator="!="},
                });
                foreach(ProcessFlowDetail DetailModel in OldData.NotEmpty())
                {
                    DetailModel.model_operation = "delete";
                    Data.Add(new ModelParameter { ValidateModel = null, SaveModel = DetailModel });
                }
                foreach (ProcessFlow FlowModel in FLowList)
                {
                    Data.Add(new ModelParameter { ValidateModel = null, SaveModel = new ProcessFlowDetail { 
                        process_code=_process,
                        user_code= FlowModel.user_code,
                        for_user_code=FlowModel.for_user_code,
                        levels=FlowModel.levels,
                        rights=FlowModel.rights,
                        module_code= _code
                    } });
                }
                return Data;
            }
            return null;

        }
        public void CompareRule()
        {            
            
            _query = new QueryParam
            {
                Table = "approval_rule_group",
                Fields = "approval_rule_group.approval_rule_group_code,process_code,priority,approval_rule.approval_rule_code,rule_table,propertyname,propertyvalue",
                Join = new List<JoinParameter>
                {
                    new JoinParameter{table="approval_rule_group_detail",condition="approval_rule_group_detail.approval_rule_group_code=approval_rule_group.approval_rule_group_code"},
                    new JoinParameter{table="approval_rule",condition="approval_rule.approval_rule_code=approval_rule_group_detail.approval_rule_code"}
                },
                Where = new List<ConditionParameter>
                {
                    new ConditionParameter{PropertyName="process_code",PropertyValue=_process}
                },
                OrderBy = "priority"
            };
            List<ProcessApprovalRule> RuleList = _repo.FindAll<ProcessApprovalRule>(_query).ToList();
            decimal[] GroupList = RuleList.Select(x => x.priority).Distinct().ToArray();
            string condition;
            if (GroupList.Count() > 0)
            {
                foreach (int code in GroupList.NotEmpty())
                {
                    bool flag = false;
                    List<ProcessApprovalRule> grouplist = RuleList.Where(x => x.priority == code).ToList();
                    foreach (ProcessApprovalRule rule in grouplist)
                    {
                        condition = $"{rule.propertyname}{rule.rule_operator}{rule.propertyvalue}";
                        int cnt = _SaveModelList.Where(x => x.GetType().GetTableName().Trim() == rule.rule_table.Trim()).AsQueryable().Where(condition).Count();
                        if (cnt <= 0)
                        {
                            flag = false;
                            break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {                        
                        //check approval_rule_group_code with for_user_code
                        FLowList = UserLevelList(UserData.user_code, code.ToString(), true);
                        if (FLowList == null)
                        {
                            //check approval_rule_group_code with for_user_code=0
                            FLowList = UserLevelList("0", code.ToString(), true);
                            if (FLowList == null)
                            {
                                //check level for for_user_code
                                FLowList = UserLevelList(UserData.user_code);
                                if (FLowList == null)
                                {
                                    //check level for for_user_code=0
                                    FLowList = UserLevelList();
                                }
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                //check level for for_user_code
                FLowList = UserLevelList(UserData.user_code);
                if (FLowList == null)
                {
                    //check level for for_user_code=0
                    FLowList = UserLevelList();
                }
            }
        }

        private List<ProcessFlow> UserLevelList(string code = "0",string rule_code="0",bool rule_flag=false)
        {
            _query = new QueryParam
            {
                Table = "process_flow",
                Fields = "user_code,levels,rights,for_user_code",
                Where = new List<ConditionParameter>
                            {
                                new ConditionParameter{PropertyName="for_user_code",PropertyValue=code},
                                new ConditionParameter{PropertyName="process_code",PropertyValue=_process}
                            },
                OrderBy = "levels"
            };
            if (rule_flag)
            {
                _query.Where.Add(new ConditionParameter { PropertyName = "approval_rule_group_code", PropertyValue = rule_code });
            }
            return _repo.FindAll<ProcessFlow>(_query).ToList();
        }
    }
}

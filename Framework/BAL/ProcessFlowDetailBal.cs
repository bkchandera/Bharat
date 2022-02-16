using FluentValidation;
using FluentValidation.Results;
using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Library.Helper;
using Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Framework.BAL
{
    public class ProcessFlowDetailBal
    {
        public ProcessFlowDetailBal() { }

        public decimal MaxLevel(int process_code,int module_code, DBRepository NewRepo)
        {
            return NewRepo.Find<decimal>(new QueryParam
            {
                Fields = "max(levels) as max_level",
                Table = "process_flow_detail",
                Where = new List<ConditionParameter>
                {
                    new ConditionParameter{PropertyName="process_code",PropertyValue=process_code},
                    new ConditionParameter{PropertyName="module_code",PropertyValue=module_code},
                }
            });
        }
        public decimal FlowDetail(ref List<ModelParameter> _data, DBRepository NewRepo, int code, string remarks)
        {
            ProcessFlowDetail FlowModel = NewRepo.FindByKey<ProcessFlowDetail>(code);
            FlowModel.approval_remarks = remarks;
            FlowModel.model_operation = "update";
            _data.Add(new ModelParameter { ValidateModel = null, SaveModel = FlowModel });
            decimal level = NewRepo.Find<int>(new QueryParam
            {
                Fields = "levels",
                Table = "process_flow_detail",
                Where = new List<ConditionParameter>
                {
                    new ConditionParameter{PropertyName="module_code",PropertyValue=FlowModel.module_code },
                    new ConditionParameter{PropertyName="module_name",PropertyValue=FlowModel.module_name },
                    new ConditionParameter{PropertyName="rights",PropertyValue="update" },
                    new ConditionParameter{PropertyName = "levels",PropertyValue=FlowModel.levels,Operator=">" }
                },
                Offset = 1,
                Limit = 1
            });
            if (level == 0)
            {
                return FlowModel.levels + 1;
            }
            return level;
        }
        public bool FlowDetail<T, T1>(T OldModel, T NewModel, DBRepository NewRepo, List<T1> OtherModel = null, List<ModelParameter> OtherModelValid = null) where T : WorkFlowModel where T1 : BaseModel
        {
            try
            {
                List<BaseModel> SaveModelList = this.SetData(OldModel, NewModel, NewRepo, OtherModelValid);
                if (SaveModelList == null)
                    return false;
                if (OtherModel != null && OtherModel.Count > 0)
                {
                    SaveModelList.AddRange(OtherModel);
                }
                NewRepo.AUDOperation(SaveModelList);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool FlowDetail<T>(T OldModel, T NewModel, DBRepository NewRepo, List<BaseModel> OtherModel = null, List<ModelParameter> OtherModelValid = null) where T : WorkFlowModel
        {
            try
            {
                List<BaseModel> SaveModelList = this.SetData(OldModel, NewModel, NewRepo, OtherModelValid);
                if (SaveModelList == null)
                    return false;
                if (OtherModel != null && OtherModel.Count > 0)
                {
                    SaveModelList.AddRange(OtherModel);
                }
                NewRepo.AUDOperation(SaveModelList);
                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }

        private List<BaseModel> SetData(WorkFlowModel OldModel, WorkFlowModel NewModel, DBRepository NewRepo, List<ModelParameter> OtherModelValid = null)
        {

            string status = string.Empty;
            decimal level = NewModel.levels;
            List<BaseModel> SaveModelList = new List<BaseModel>();

            //if (OldModel.status != "Drafted")
            //    status = OldModel.status.Trim() == "Rejected" ? "Rejected" : OldModel.status.Trim() == "Closed" ? "Closed" : OldModel.status.Trim() == "Cancelled" ? "Cancelled" : "Pending";

            //if (NewModel.status == "Drafted")
            //{
            //    status = "Cancelled";
            //    ModelInfo _property = NewModel.GetProperty("is_draft");
            //    _property.Property.SetValue(NewModel, false, null);
            //}  

            status = new string[] { "Rejected", "Closed", "Cancelled" }.Contains(OldModel.status.Trim()) ? OldModel.status.Trim() : "Pending";

            if (status == "Pending" || status == "Rejected")
            {
                string module_code = NewModel.GetPrimaryKey().PropertyValue.ToString();
                List<ProcessFlowDetail> FlowModelList = NewRepo.FindAll<ProcessFlowDetail>(new List<ConditionParameter> {
                    new ConditionParameter{PropertyName="module_code",PropertyValue=module_code },
                    new ConditionParameter{PropertyName="process_code",PropertyValue=NewModel.process_code },
                    new ConditionParameter{PropertyName="levels",PropertyValue=NewModel.levels },                    
                });
                foreach(ProcessFlowDetail FlowModel in FlowModelList.NotEmpty())
                {
                    FlowModel.is_approve = true;
                    FlowModel.model_operation = "update";
                    FlowModel.process_status = OldModel.status;
                    if (FlowModel.user_code== UserData.user_code)
                        FlowModel.approval_remarks = OldModel.approval_remarks;
                    SaveModelList.Add(FlowModel);
                }
                if (FlowModelList != null && FlowModelList.Count()>0)
                {
                   
                    level = NewRepo.Find<decimal>(new QueryParam
                    {
                        Fields = "levels",
                        Table = "process_flow_detail",
                        Where = new List<ConditionParameter>
                        {
                            new ConditionParameter{PropertyName="module_code",PropertyValue=module_code },
                            new ConditionParameter{PropertyName="process_code",PropertyValue=NewModel.process_code },
                            new ConditionParameter{PropertyName = "levels",PropertyValue=NewModel.levels,Operator=">" }
                        },
                        Offset = 1,
                        Limit = 1
                    });
                    if (level == 0)
                    {
                        level = FlowModelList.LastOrDefault().levels + 1;
                        if(status == "Pending")
                            status = "Approved";
                    }
                }
            }
            else
            {
                ProcessFlowDetail FlowModel = new ProcessFlowDetail
                {
                     process_code = NewModel.process_code,
                     user_code = UserData.user_code,
                    levels=-1,
                    rights= "update",
                    for_user_code=NewModel.created_by,
                    module_code= NewModel.GetPrimaryKey().PropertyValue.ToString(),
                    approval_remarks = OldModel.approval_remarks,
                    is_approve=true,
                    process_status = status
                };
                SaveModelList.Add(FlowModel);
            }
            



            NewModel.levels = level;
            NewModel.approval_remarks = OldModel.approval_remarks;
            NewModel.status = status;
            NewModel.model_operation = "update";
            SaveModelList.Add(NewModel);
            if (OtherModelValid != null && OtherModelValid.Count > 0)
            {
                foreach (ModelParameter param in OtherModelValid.Where(x => x.ValidateModel != null))
                {
                    ValidationResult result = param.ValidateModel.Validate(param.SaveModel);
                    if (!result.IsValid)
                    {
                        return null;
                    }
                }
                SaveModelList.AddRange(OtherModelValid.Select(x => x.SaveModel));
            }
            return SaveModelList;


        }
    }
}

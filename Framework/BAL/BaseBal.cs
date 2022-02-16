using FluentValidation;
using FluentValidation.Results;
using Framework.CustomDataType;
using Framework.DataAccess.Dapper;
using Framework.Extension;
using Framework.Library.Attribute;
using Framework.Library.Helper;
using Framework.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Framework.BAL
{
    //test
    public class BaseBal<T> where T : class
    {
        protected DBRepository NewRepo = new DBRepository();

        protected ValidationResult IsValid(IValidator s, IBaseModel item)
        {
            return s.Validate(item);
        }


        //operation 0 = Insert
        // operation 1 = Update
        // operation 2= Delete
        protected string SingleRecordTransaction(List<ModelParameter> ObjectList, int Operation = 0)
        {
            try
            {
                string pk;
                if (Operation != 2)
                {
                    string[] cutom_error_table = new string[2] { "member_status", "member_share_refund" };
                    foreach (ModelParameter data in ObjectList)
                    {
                        if (data.ValidateModel != null && data.SaveModel.model_operation != "delete")
                        {
                            ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                            if (!result.IsValid)
                            {
                                string val = data.SaveModel.custom_edit;
                                if (val.NotEmpty())
                                {
                                    QueryParam Query = new QueryParam
                                    {
                                        Table = "approval_transaction",
                                        Fields = $"error='{result.GetErrors(true)}'",
                                        Where = new List<ConditionParameter>
                                    {
                                        Condition("approval_transaction_code",val)
                                    }
                                    };
                                    NewRepo.Update(Query);
                                }
                                return "error";
                            }
                        }

                    }
                }
                switch (Operation)
                {
                    case 0:
                        pk = NewRepo.Add(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>());
                        break;
                    case 1:
                        pk = NewRepo.Update(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>()).ToString();
                        break;
                    case 2:
                        pk = NewRepo.Delete(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>()).ToString();
                        break;
                    case 3:
                        pk = NewRepo.AUDOperation(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>()).ToString();
                        break;
                    default:
                        pk = "";
                        break;
                }
                if (pk.NotEmpty())
                {
                    return "success";
                }
                else
                {
                    return "error";
                }

            }
            catch (Exception ex)
            {
                return "error";
            }
        }
        protected IActionResult BulkSave(List<ModelParameter> SaveList, List<ModelParameter> BulkList)
        {
            try
            {
                foreach (var data in SaveList)
                {
                    ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                    if (!result.IsValid)
                    {
                        return new CustomResult("validation_error", result.GetErrors());
                    }
                }
                foreach (var data in BulkList)
                {
                    ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                    if (!result.IsValid)
                    {
                        return new CustomResult("validation_error", result.GetErrors());
                    }
                }
                DataTable dataTable = new DataTable();
                //Get all the properties by using reflection   
                List<IBaseModel> TmpBulk = BulkList.Select(e => e.SaveModel).ToList<IBaseModel>();

                PropertyInfo[] Props = TmpBulk.FirstOrDefault().GetType().GetColumnsInfo();
                List<string> ColumnMapping = new List<string>();
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    // dataTable.Columns.Add(prop.Name,prop.PropertyType);
                    dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    ColumnMapping.Add(prop.Name);
                }
                foreach (var item in TmpBulk)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                List<IBaseModel> save = SaveList.Select(e => e.SaveModel).ToList<IBaseModel>();
                NewRepo.BulkAdd(save, TmpBulk.FirstOrDefault().GetType().GetTableName(), dataTable, ColumnMapping);
                return new CustomResult("success");
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }
        protected CustomResult SaveData(List<ModelParameter> ObjectList, bool IsReturn = false, bool is_workflow = true)
        {
            try
            {
                string pk;
                foreach (var data in ObjectList)
                {
                    ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                    if (!result.IsValid)
                    {
                        return new CustomResult("validation_error", result.GetErrors());
                    }
                }
                if (is_workflow)
                    AddWorkflow(ref ObjectList);
                pk = NewRepo.Add(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>());
                if (IsReturn)
                {
                    return new CustomResult("success", new ReturnPK { code = pk });
                }
                else
                {
                    return new CustomResult();
                }
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }


        protected CustomResult UpdateData(List<ModelParameter> ObjectList)
        {
            try
            {
                foreach (ModelParameter data in ObjectList)
                {

                    data.SaveModel.model_operation = "update";
                    if (data.ValidateModel != null)
                    {
                        ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                        if (!result.IsValid)
                        {
                            return new CustomResult("validation_error", result.GetErrors());
                        }
                    }

                }
                NewRepo.Update(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>());
                return new CustomResult("success");
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }
        protected IActionResult AddUpdate(List<ModelParameter> ObjectList)
        {
            try
            {
                foreach (var data in ObjectList)
                {
                    ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                    if (!result.IsValid)
                    {
                        return new CustomResult("validation_error", result.GetErrors());
                    }
                }
                NewRepo.AUDOperation(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>());
                return new CustomResult("success");
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }
        protected CustomResult AUDOperation(List<ModelParameter> ObjectList, bool is_workflow = true)
        {
            try
            {
                foreach (ModelParameter data in ObjectList)
                {
                    if (data.SaveModel.model_operation != "delete" && data.ValidateModel != null)
                    {
                        ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                        if (!result.IsValid)
                        {
                            return new CustomResult("validation_error", result.GetErrors());
                        }
                    }
                }
                if (is_workflow)
                    AddWorkflow(ref ObjectList);
                NewRepo.AUDOperation(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>());
                return new CustomResult("success");
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }
        protected IActionResult DeleteData(List<BaseModel> ObjectList)
        {
            try
            {
                NewRepo.Delete(ObjectList);
                return new CustomResult("success");
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }
        protected IActionResult DropInsert(List<ModelParameter> ObjectList, QueryParam DeleteData)
        {
            try
            {
                if (NewRepo.Delete(DeleteData))
                {
                    foreach (var data in ObjectList)
                    {
                        ValidationResult result = IsValid(data.ValidateModel, data.SaveModel);
                        if (!result.IsValid)
                        {
                            return new CustomResult("validation_error", result.GetErrors());
                        }
                    }
                    NewRepo.AUDOperation(ObjectList.Select(e => e.SaveModel).ToList<BaseModel>());
                    return new CustomResult("success");
                }
                else
                {
                    return new CustomResult("error", "Data Not Deleted");
                }
            }
            catch (Exception E)
            {
                return new CustomResult("exception", E.Message);
            }
        }

        protected ConditionParameter Condition(string _PropertyName, dynamic _PropertyValue, string _Operator = "=")
        {
            return new ConditionParameter
            {
                PropertyName = _PropertyName,
                PropertyValue = _PropertyValue,
                Operator = _Operator
            };
        }
        private void AddWorkflow(ref List<ModelParameter> data)
        {

            List<BaseModel> SaveList = data.Select(x => x.SaveModel).ToList();
            WorkFlowModel _model = SaveList.Where(x => x.GetType().GetCustomAttribute<WorkFlowAttribute>() != null).Select(x => (WorkFlowModel)x).FirstOrDefault();

            if (_model != null && _model.status != "Drafted" && _model.model_operation.ToLower() != "delete")
            {
                int processcode = _model.GetType().GetCustomAttribute<WorkFlowAttribute>().Code;
                WorkFlow WorkFlowModel = new WorkFlow();
                decimal level = 0;
                string pk = _model.GetPrimaryKey().PropertyValue.ToString();
                List<ModelParameter> WorkFlowData = WorkFlowModel.SaveWorkFlow(SaveList, processcode, pk, ref level);
                for (int i = 0; i < data.Count(); i++)
                {
                    if (data[i].SaveModel.GetType().GetCustomAttribute<WorkFlowAttribute>() != null)
                    {
                        ModelInfo _property = data[i].SaveModel.GetProperty("process_code");
                        _property.Property.SetValue(data[i].SaveModel, processcode, null);
                        if (WorkFlowData != null)
                        {
                            _property = data[i].SaveModel.GetProperty("levels");
                            _property.Property.SetValue(data[i].SaveModel, level, null);
                            _property = data[i].SaveModel.GetProperty("status");
                            _property.Property.SetValue(data[i].SaveModel, "Pending", null);
                            data.AddRange(WorkFlowData);
                        }
                        break;
                    }
                }
            }

        }
    }




}

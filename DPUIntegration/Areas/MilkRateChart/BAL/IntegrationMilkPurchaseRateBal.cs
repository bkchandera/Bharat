using System.Collections.Generic;
using DPUIntegration.Areas.MilkRateChart.Models;
using Framework.BAL;
using Framework.CustomDataType;
using System.Data;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace DPUIntegration.Areas.MilkRateChart.BAL
{
    public class IntegrationMilkPurchaseRateBal : BaseBal<IntegrationMilkPurchaseRateDetail>
    {
        public IntegrationMilkPurchaseRateBal() { }

        public ExcelPackage GenerateOfflineRateChart(string rate_code, string station_code, string wef_date, string shift_short_code, bool flag = false)
        {
            ExcelPackage package = new OfficeOpenXml.ExcelPackage();

            QueryParam Query = new QueryParam
            {
                Fields = "purchase_rate_code,milk_type.milk_type_code,milk_type_name,rate_type_name,milk_quality_type_name",
                Distinct = "distinct",
                Join = new List<JoinParameter>
                    {
                        new JoinParameter{table="milk_type",condition="milk_type.milk_type_code=integration_milk_purchase_rate_base.milk_type_code"},
                        new JoinParameter{table="milk_quality_type",condition="milk_quality_type.milk_quality_type_code=integration_milk_purchase_rate_base.milk_quality_type_code"}
                    },
                Where = new List<ConditionParameter>
                    {
                        Condition("purchase_rate_code",rate_code)
                    }
            };
            IEnumerable<IntegrationMilkPurchaseRateBase> MilkTypeList = NewRepo.FindAll<IntegrationMilkPurchaseRateBase>(Query);
            ExcelWorksheet worksheet;
            foreach (IntegrationMilkPurchaseRateBase BaseModel in MilkTypeList)
            {
                string code = (flag) ? $"~{BaseModel.purchase_rate_code}" : "";
                worksheet = package.Workbook.Worksheets.Add($"{BaseModel.milk_type_name}");
                Query = new QueryParam
                {
                    Sp = "sp_integration_rate_chart_view",
                    Where = new List<ConditionParameter>
                        {
                            Condition("purchase_rate_code",BaseModel.purchase_rate_code),
                            Condition("milk_type_code",BaseModel.milk_type_code),
                            Condition("rate_type_name",BaseModel.rate_type_name),
                        }
                };
                var json = JsonConvert.SerializeObject(NewRepo.FindAll(Query));
                DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                worksheet.Cells.LoadFromDataTable(dataTable, true);
            }

            worksheet = package.Workbook.Worksheets.Add($"EffectiveDate");
            worksheet.Cells[1, 1].Value = "station_code";
            worksheet.Cells[1, 2].Value = "rate_effective_date";
            worksheet.Cells[1, 3].Value = "rate_effective_shift";
            worksheet.Cells[2, 1].Value = station_code;
            worksheet.Cells[2, 2].Value = wef_date;
            worksheet.Cells[2, 3].Value = shift_short_code;

            return package;
        }
    }
}

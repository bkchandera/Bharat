using System.Collections.Generic;
using System.Dynamic;
using DPUIntegration.Areas.MilkSummary.Models;
using Framework.BAL;
using Framework.CustomDataType;
using Framework.Extension;
using Framework.Library.Helper;
using Microsoft.AspNetCore.Mvc;

namespace DPUIntegration.Areas.MilkSummary.BAL
{
    public class IntegrationMilkSummaryBal : BaseBal<IntegrationMilkSummary>
    {
        #region Property  
        private List<ModelParameter> _data;
        #endregion

        #region Constructor

        IntegrationMilkSummary _integrationMilkSummary = new IntegrationMilkSummary();
        public IntegrationMilkSummaryBal() { }

        #endregion

        public IActionResult SaveMultiple(List<TmpIntegrationMilkSummary> TmpMilkSummaryList)
        {
            List<dynamic> rescollection = new List<dynamic>();
            foreach (TmpIntegrationMilkSummary TmpMilkSummaryObj in TmpMilkSummaryList)
            {
                List<dynamic> _response = SetData(TmpMilkSummaryObj);
                rescollection.Add(_response);
            }
            return new CustomResult("success", rescollection);
        }

        private List<dynamic> SetData(TmpIntegrationMilkSummary TmpMilkSummaryObj)
        {
            _integrationMilkSummary = TmpMilkSummaryObj.Parse<IntegrationMilkSummary, TmpIntegrationMilkSummary>();
            List<dynamic> rescollection = new List<dynamic>();

            foreach (var item in TmpMilkSummaryObj.summary_data)
            {
                _data = new List<ModelParameter>();
                _integrationMilkSummary = new IntegrationMilkSummary();
                _integrationMilkSummary.bmc_code = TmpMilkSummaryObj.bmc_code;
                _integrationMilkSummary.transaction_date = TmpMilkSummaryObj.transaction_date;
                _integrationMilkSummary.collection_date = TmpMilkSummaryObj.collection_date;
                _integrationMilkSummary.shift_code = TmpMilkSummaryObj.shift_code;
                _integrationMilkSummary.mpp_code = item.mpp_code;
                _integrationMilkSummary.qty = item.qty;
                _integrationMilkSummary.total_amount = item.total_amount;
                _integrationMilkSummary.sample_date_time = item.sample_date_time;
                _integrationMilkSummary.cow_total_count = item.cow_total_count;
                _integrationMilkSummary.buffalo_total_count = item.buffalo_total_count;
                _integrationMilkSummary.mix_total_count = item.mix_total_count;
                _integrationMilkSummary.total_sample_count = item.total_sample_count;

                _data.Add(new ModelParameter { ValidateModel = new IntegrationMilkSummaryValidator(), SaveModel = _integrationMilkSummary });

                dynamic response = new ExpandoObject();
                var actionResult = AddUpdate();

                ReturnResponse returnResponse = actionResult.GetPropValue("_result");
                response.station_code = item.mpp_code;
                response.date = TmpMilkSummaryObj.transaction_date;
                response.shift = TmpMilkSummaryObj.shift_code;
                response.datacount = TmpMilkSummaryObj.data_count;
                response.mpp_summary_code = _integrationMilkSummary.Id;
                response.status = returnResponse.code;
                response.status_message = returnResponse.message;
                response.error_msg = returnResponse.code == 200 ? string.Empty : returnResponse.data;
                rescollection.Add(response);
            }
            return rescollection;
        }
        public IActionResult AddUpdate()
        {
            return AddUpdate(_data);
        }
    }
}

using System.Collections.Generic;
using System.Dynamic;
using Framework.BAL;
using Framework.CustomDataType;
using Framework.Extension;
using Framework.Library.Helper;
using Microsoft.AspNetCore.Mvc;
using DPUIntegration.Areas.MilkDispatch.Models;

namespace DPUIntegration.Areas.MilkDispatch.BAL
{
    public class IntegrationMppDispatchBal : BaseBal<IntegrationMppDispatch>
    {
        #region Property  
        private List<ModelParameter> _data;
        #endregion

        #region Constructor

        IntegrationMppDispatch integrationMppDispatch = new IntegrationMppDispatch();
        public IntegrationMppDispatchBal() { }

        #endregion

        public IActionResult SaveMultiple(List<TmpIntegrationMppDispatch> TmpDispatchList)
        {
            List<dynamic> rescollection = new List<dynamic>();
            foreach (TmpIntegrationMppDispatch TmpDispatchObj in TmpDispatchList)
            {
                List<dynamic> _response = SetData(TmpDispatchObj);
                rescollection.Add(_response);
            }
            return new CustomResult("success", rescollection);
        }

        private List<dynamic> SetData(TmpIntegrationMppDispatch TmpDispatchObj)
        {
            integrationMppDispatch = TmpDispatchObj.Parse<IntegrationMppDispatch, TmpIntegrationMppDispatch>();
            List<dynamic> rescollection = new List<dynamic>();

            foreach (var item in TmpDispatchObj.dispatch_data)
            {
                _data = new List<ModelParameter>();
                integrationMppDispatch = new IntegrationMppDispatch();
                integrationMppDispatch.bmc_code = TmpDispatchObj.bmc_code;
                integrationMppDispatch.transaction_date = TmpDispatchObj.transaction_date;
                integrationMppDispatch.collection_date = TmpDispatchObj.collection_date;
                integrationMppDispatch.shift_code = TmpDispatchObj.shift_code;
                integrationMppDispatch.mpp_code = item.mpp_code;
                integrationMppDispatch.milk_type = item.milk_type;
                integrationMppDispatch.sample_no = item.sample_no;
                integrationMppDispatch.qty = item.qty;
                integrationMppDispatch.fat = item.fat;
                integrationMppDispatch.snf = item.snf;
                integrationMppDispatch.water = item.water;
                integrationMppDispatch.sample_time = item.sample_time;
                integrationMppDispatch.rate = item.rate;
                integrationMppDispatch.amount = item.rate;

                _data.Add(new ModelParameter { ValidateModel = new IntegrationMppDispatchValidator(), SaveModel = integrationMppDispatch });

                dynamic response = new ExpandoObject();
                var actionResult = AddUpdate();

                ReturnResponse returnResponse = actionResult.GetPropValue("_result");
                response.station_code = item.mpp_code;
                response.date = TmpDispatchObj.transaction_date;
                response.shift = TmpDispatchObj.shift_code;
                response.datacount = TmpDispatchObj.data_count;
                response.mpp_dispatch_code = integrationMppDispatch.mpp_dispatch_code;
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

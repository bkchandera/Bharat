using System.Collections.Generic;
using System.Dynamic;
using DPUIntegration.Areas.MppCollection.Models;
using Framework.BAL;
using Framework.CustomDataType;
using Framework.Extension;
using Framework.Library.Helper;
using Microsoft.AspNetCore.Mvc;

namespace DPUIntegration.Areas.MppCollection.BAL
{
    public class IntegrationMppCollectionBal : BaseBal<IntegrationMppCollection>
    {
        #region Property  
        private List<ModelParameter> _data;
        #endregion

        #region Constructor

        IntegrationMppCollection _integrationMppCollection = new IntegrationMppCollection();
        public IntegrationMppCollectionBal() { }

        #endregion

        public IActionResult SaveMultiple(List<TmpIntegrationMppCollection> TmpCollectionList)
        {
            List<dynamic> rescollection = new List<dynamic>();
            foreach (TmpIntegrationMppCollection TmpCollectionObj in TmpCollectionList)
            {
                List<dynamic> _response = SetData(TmpCollectionObj);
                rescollection.Add(_response);
            }
            return new CustomResult("success", rescollection);
        }

        private List<dynamic> SetData(TmpIntegrationMppCollection TmpCollectionObj)
        {
            _integrationMppCollection = TmpCollectionObj.Parse<IntegrationMppCollection, TmpIntegrationMppCollection>();
            List<dynamic> rescollection = new List<dynamic>();

            foreach (var item in TmpCollectionObj.collection_data)
            {
                _data = new List<ModelParameter>();
                _integrationMppCollection = new IntegrationMppCollection();
                _integrationMppCollection.bmc_code = TmpCollectionObj.bmc_code;
                _integrationMppCollection.mpp_code = TmpCollectionObj.mpp_code;
                _integrationMppCollection.transaction_date = TmpCollectionObj.transaction_date;
                _integrationMppCollection.collection_date = TmpCollectionObj.collection_date;
                _integrationMppCollection.shift_code = TmpCollectionObj.shift_code;
                _integrationMppCollection.farmer_code = item.farmer_code;
                _integrationMppCollection.milk_type = item.milk_type;
                _integrationMppCollection.sample_no = item.sample_no;
                _integrationMppCollection.qty = item.qty;
                _integrationMppCollection.fat = item.fat;
                _integrationMppCollection.snf = item.snf;
                _integrationMppCollection.water = item.water;
                _integrationMppCollection.sample_time = item.sample_time;
                _integrationMppCollection.rate = item.rate;
                _integrationMppCollection.amount = item.rate;

                _data.Add(new ModelParameter { ValidateModel = new IntegrationMppCollectionValidator(), SaveModel = _integrationMppCollection });

                dynamic response = new ExpandoObject();
                var actionResult = AddUpdate();

                ReturnResponse returnResponse = actionResult.GetPropValue("_result");
                response.station_code = TmpCollectionObj.mpp_code;
                response.date = TmpCollectionObj.transaction_date;
                response.shift = TmpCollectionObj.shift_code;
                response.datacount = TmpCollectionObj.data_count;
                response.mpp_collection_code = _integrationMppCollection.mpp_collection_code;
                response.status = returnResponse.code;
                response.message = returnResponse.message;
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

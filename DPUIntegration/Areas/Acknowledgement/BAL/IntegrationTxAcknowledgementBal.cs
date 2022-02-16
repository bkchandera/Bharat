using System.Collections.Generic;
using Framework.BAL;
using Framework.CustomDataType;
using Framework.Extension;
using Framework.Library.Helper;
using Microsoft.AspNetCore.Mvc;
using DPUIntegration.Areas.Acknowledgement.Models;
using System.Dynamic;

namespace DPUIntegration.Areas.Acknowledgement.BAL
{
    public class IntegrationTxAcknowledgementBal : BaseBal<IntegrationTxAcknowledgement>
    {
        #region Property  
        private List<ModelParameter> _data;
        #endregion

        #region Constructor

        IntegrationTxAcknowledgement integrationTxAcknowledgement = new IntegrationTxAcknowledgement();
        public IntegrationTxAcknowledgementBal() { }

        #endregion

        public IActionResult SaveMultiple(List<TmpIntegrationTxAcknowledgement> TmpAcknowledgeList)
        {
            List<dynamic> rescollection = new List<dynamic>();
            foreach (TmpIntegrationTxAcknowledgement TmpCollectionObj in TmpAcknowledgeList)
            {
                List<dynamic> _response = SetData(TmpCollectionObj);
                rescollection.Add(_response);
            }
            return new CustomResult(null, rescollection);
        }

        private List<dynamic> SetData(TmpIntegrationTxAcknowledgement TmpAcknowldgeObj)
        {
            integrationTxAcknowledgement = TmpAcknowldgeObj.Parse<IntegrationTxAcknowledgement, TmpIntegrationTxAcknowledgement>();
            List<dynamic> rescollection = new List<dynamic>();

            foreach (var item in TmpAcknowldgeObj.acknowledgement_data)
            {
                _data = new List<ModelParameter>();
                integrationTxAcknowledgement = new IntegrationTxAcknowledgement();
                integrationTxAcknowledgement.bmc_code = TmpAcknowldgeObj.bmc_code;
                integrationTxAcknowledgement.mpp_code = item.mpp_code;
                integrationTxAcknowledgement.transaction_date = TmpAcknowldgeObj.transaction_date;
                integrationTxAcknowledgement.download_date = item.download_date;
                integrationTxAcknowledgement.key_name = item.key_name;
                integrationTxAcknowledgement.key_value = string.Empty;
                if (item.key_name == "Rate")
                    integrationTxAcknowledgement.key_value = string.Concat(item.rate_code, item.milk_type);
                integrationTxAcknowledgement.download_success = item.download_success;

                _data.Add(new ModelParameter { ValidateModel = new IntegrationTxAcknowledgementValidator(), SaveModel = integrationTxAcknowledgement });

                dynamic response = new ExpandoObject();
                var actionResult = AddUpdate();

                ReturnResponse returnResponse = actionResult.GetPropValue("_result");
                response.station_code = item.mpp_code;
                response.date = item.download_date;
                response.Ack_Id = integrationTxAcknowledgement.Id;
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

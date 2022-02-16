using Microsoft.AspNetCore.Mvc;
using Framework.Controllers;
using Framework.Extension;
using DPUIntegration.Areas.Acknowledgement.BAL;
using DPUIntegration.Areas.Acknowledgement.Models;

namespace DPUIntegration.Areas.Acknowledgement.Controllers
{

    [Area("Acknowledgement")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class IntegrationTxAcknowledgementController : BaseController
    {
        public IntegrationTxAcknowledgementBal _integrationTxAcknowledgementBal;

        [HttpPost]
        [Route("integration_tx_aknowledgement")]
        public IActionResult TxAknowledgement([FromBody] object data)
        {
            _integrationTxAcknowledgementBal = new IntegrationTxAcknowledgementBal();
            return _integrationTxAcknowledgementBal.SaveMultiple(data.ParseRequestList<TmpIntegrationTxAcknowledgement>());
        }
    }
}

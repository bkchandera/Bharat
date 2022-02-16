using Microsoft.AspNetCore.Mvc;
using Framework.Controllers;
using Framework.Extension;
using DPUIntegration.Areas.MilkSummary.BAL;
using DPUIntegration.Areas.MilkSummary.Models;

namespace DPUIntegration.Areas.MilkSummary.Controllers
{
    [Area("MilkShiftSummary")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class IntegrationMilkSummaryController : BaseController
    {
        public IntegrationMilkSummaryBal _integrationMilkSummaryBal;

        [HttpPost]
        [Route("integrationmilksummary")]
        public IActionResult MilkSummary([FromBody] object data)
        {
            _integrationMilkSummaryBal = new IntegrationMilkSummaryBal();
            return _integrationMilkSummaryBal.SaveMultiple(data.ParseRequestList<TmpIntegrationMilkSummary>());
        }
    }
}

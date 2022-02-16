using Microsoft.AspNetCore.Mvc;
using Framework.Controllers;
using Framework.Extension;
using DPUIntegration.Areas.MilkDispatch.BAL;
using DPUIntegration.Areas.MilkDispatch.Models;

namespace DPUIntegration.Areas.MilkDispatch.Controllers
{

    [Area("MppDispatch")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class IntegrationMppDispatchController : BaseController
    {
        public IntegrationMppDispatchBal _integrationMppDispatchBal;

        [HttpPost]
        [Route("integrationmilkdispatch")]
        public IActionResult MilkDispatch([FromBody] object data)
        {
            _integrationMppDispatchBal = new IntegrationMppDispatchBal();
            return _integrationMppDispatchBal.SaveMultiple(data.ParseRequestList<TmpIntegrationMppDispatch>());
        }
    }
}

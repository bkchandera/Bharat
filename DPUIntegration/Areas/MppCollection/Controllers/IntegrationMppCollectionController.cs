using Framework.Controllers;
using Framework.Extension;
using Microsoft.AspNetCore.Mvc;
using DPUIntegration.Areas.MppCollection.BAL;
using DPUIntegration.Areas.MppCollection.Models;

namespace DPUIntegration.Areas.MppCollection.Controllers
{

    [Area("MppCollection")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class IntegrationMppCollectionController : BaseController
    {

        public IntegrationMppCollectionBal _integrationMppCollectionBal;

        [HttpPost]
        [Route("integrationmilkcollection")]
        public IActionResult MilkCollection([FromBody] object data)
        {
            _integrationMppCollectionBal = new IntegrationMppCollectionBal();
            return _integrationMppCollectionBal.SaveMultiple(data.ParseRequestList<TmpIntegrationMppCollection>());
        }

    }
}

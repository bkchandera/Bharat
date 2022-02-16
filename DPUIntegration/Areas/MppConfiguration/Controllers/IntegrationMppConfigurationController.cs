using Framework.Controllers;
using Framework.Extension;
using Microsoft.AspNetCore.Mvc;

namespace DPUIntegration.Areas.MppConfiguration.Controllers
{
    [Area("MppConfiguration")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class IntegrationMppConfigurationController : BaseController
    {
        [HttpPost]
        [Route("list")]
        public IActionResult List([FromBody] object data)
        {
            ListEngine listEngine = new ListEngine();
            SetParam();
            return listEngine.List(data, FileName, "get_mpp_configuration");
        }

        [HttpPost]
        [Route("memberlist")]
        public IActionResult MemberInfoList([FromBody] object data)
        {
            ListEngine listEngine = new ListEngine();
            SetParam();
            return listEngine.List(data, FileName, "get_mpp_member_download");
        }


        //[HttpPost]
        //[Route("offline_memberlist")]
        //public IActionResult OfflineMemberList([FromBody] object data)
        //{
        //    ListEngine listEngine = new ListEngine();
        //    SetParam();
        //    return listEngine.List(data, FileName, "get_offline_mpp_member_list");
        //}
    }
}

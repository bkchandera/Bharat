using Microsoft.AspNetCore.Mvc;
using Framework.Controllers;
using Framework.Extension;
using DPUIntegration.Areas.MilkRateChart.BAL;

namespace DPUIntegration.Areas.MilkRateChart.Controllers
{
    [Area("MilkRateChart")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class IntegrationMilkPurchaseRateController : BaseController
    {
        public IntegrationMilkPurchaseRateBal _integrationMilkPurchaseRateObj;

        [HttpPost]
        [Route("rateapplicability")]
        public IActionResult RateApplicabilityList([FromBody] object data)
        {
            ListEngine listEngine = new ListEngine();
            SetParam();
            return listEngine.List(data, FileName, "get_rate_applicability_list");
        }

        [HttpGet]
        [Route("rate_chart_excel")]
        public IActionResult RateChartDownload(string rate_code, string station_code, string wef_date, string shift)
        {
            _integrationMilkPurchaseRateObj = new IntegrationMilkPurchaseRateBal();
            byte[] fileContents = _integrationMilkPurchaseRateObj.GenerateOfflineRateChart(rate_code, station_code, wef_date, shift).GetAsByteArray();
            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }
            return File(fileContents: fileContents, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName: "erp_integration_rate_chart.xls");
        }
    }
}

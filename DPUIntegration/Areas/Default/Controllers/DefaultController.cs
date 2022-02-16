using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Controllers;
using Framework.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DPUIntegration.Areas.Default.Controllers
{
    [Area("Default")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1", Deprecated = true)]
    [ApiVersion("2.0")]
    public class DefaultController : BaseController
    {

        string[] authors = new string[]
        { "Jaydip Kanjilal", "Steve Smith", "Stephen Jones" };
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return authors;
        }
        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public string Get(int id)
        {
            return authors[id];
        }

        [HttpPost]
        [Route("memberinfodownload")]
        [MapToApiVersion("2.0")]
        public IActionResult memberinfodownload([FromBody] object data)
        {
            ListEngine listEngine = new ListEngine();
            SetParam();
            return listEngine.List(data, FileName, "memberinfo_list");
        }
    }
}

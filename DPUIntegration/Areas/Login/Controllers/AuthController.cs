using Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Framework.Extension;
using DPUIntegration.Areas.Login.BAL;
using DPUIntegration.Areas.Login.Models;

namespace DPUIntegration.Areas.Auth.Controllers
{
    [Area("Login")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {
        public AuthBal _authBal;

        [HttpPost]
        [Route("login")]
        public IActionResult SignIn([FromBody] object data)
        {
            _authBal = new AuthBal(data.ParseRequest<Users>());
            string tokenString = string.Empty;
            CustomResult _CustomResult = _authBal.Login(ref tokenString);
            if (_CustomResult._result.code == 200)
            {
                Response.Headers["authorization"] = tokenString;
            }
            return _CustomResult;
        }

        [HttpGet]
        [Route("redisfreeaction")]
        public IActionResult RedisFreeAction()
        {
            _authBal = new AuthBal();
            _authBal.UpdateFreeAction();
            return new CustomResult();
        }
    }
}

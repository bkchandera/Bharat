using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace Framework.Extension
{

    public class CustomResult : IActionResult
    {

        public readonly ReturnResponse _result;

        public CustomResult(string flag=null,dynamic data=null)
        {
            FlagResponse _flag = new FlagResponse();
            _result = _flag.ReturnFlag(flag);
            _result.data = data;

            //var isoConvert = new IsoDateTimeConverter();
            //isoConvert.DateTimeFormat = "dd-MM-yyyy";
            //_result.data = JsonConvert.SerializeObject(data, isoConvert);
         //   response.Write(JsonConvert.SerializeObject(Data, isoConvert));
        }
        public CustomResult(ReturnResponse result)
        {
            _result = result;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult objectResult = new ObjectResult(_result);
            await objectResult.ExecuteResultAsync(context);
        }

    }

    public class ReturnResponse
    {
        public int code;
        public string message;
        public dynamic data;
        public ReturnResponse(dynamic data = null, int code = 200, string message = "success")
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }
        public ReturnResponse(int code,string message)
        {
            this.code = code;
            this.message = message;
        }      

    }

    class FlagResponse
    {
        public ReturnResponse ReturnFlag(string flag)
        {

            switch (flag)
            {
                case "validation_error" : return new ReturnResponse(300,"Error");
                case "exception": return new ReturnResponse(700, "Error");
                case "error": return new ReturnResponse(700, "Error");
                default: return new ReturnResponse(200, "Success");

            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Framework.Library.Filter
{
    public class ExceptionActionFilter : IExceptionFilter
    {
       
        //private readonly TelemetryClient _telemetryClient;

        //public ExceptionActionFilter(
        //    IHostingEnvironment hostingEnvironment
        //    //,TelemetryClient telemetryClient
        //    )
        //{
        //    _hostingEnvironment = hostingEnvironment;
        //    //_telemetryClient = telemetryClient;
        //}

        #region Overrides of ExceptionFilterAttribute

        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            String message = String.Empty;

            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "A server error occurred.";
                status = HttpStatusCode.NotImplemented;
            }
            //else if (exceptionType == typeof(MyAppException))
            //{
            //    message = context.Exception.ToString();
            //    status = HttpStatusCode.InternalServerError;
            //}
            else
            {
                message = context.Exception.Message;
                status = HttpStatusCode.NotFound;
            }
            context.ExceptionHandled = true;

            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = (int)status;
            response.ContentType = "application/json";
            var err = message + " " + context.Exception.StackTrace;
            context.Result= new ObjectResult(new {Message = message, Data = err });
           // response.WriteAsync(err);
        }

        #endregion
    }
}

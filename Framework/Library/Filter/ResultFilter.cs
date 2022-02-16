using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Library.Filter
{
    public class ResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            //if (context.Result is ObjectResult objectResult)
            //{
            //    objectResult.Value = new ApiResult { Data = objectResult.Value };
            //}
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}

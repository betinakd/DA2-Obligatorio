using Adapter.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var statusCode = context.Exception switch
        {
            InvalidAttribute => StatusCodes.Status400BadRequest,
            InvalidLogic => StatusCodes.Status409Conflict,
            ArgumentNullException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        var errorResponse = new
        {
            Message = context.Exception.Message
        };

        context.Result = new JsonResult(errorResponse)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
using Adapter.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        try
        {
            throw context.Exception;
        }
        catch(InvalidAttribute e)
        {
            context.Result = new BadRequestObjectResult(new { Message = e.Message });
        }
        catch(ObjectNotFoundException e)
        {
            context.Result = new NotFoundObjectResult(e.Message);
        }
    }
}
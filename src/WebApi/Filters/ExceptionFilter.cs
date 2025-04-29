using System.Net;
using Adapter.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public sealed class ExceptionFilter : IExceptionFilter
{
    private readonly Dictionary<Type, Func<Exception, IActionResult>> _errorFactories = new()
{
    {
        typeof(InvalidAttributeAdapter),
        ex => new ObjectResult(new ErrorResponse
        {
            InnerCode = 1,
            Message = ex.Message
        })
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        }
    },
    {
        typeof(NonExistentValueAdapter),
        ex => new ObjectResult(new ErrorResponse
        {
            InnerCode = 2,
            Message = ex.Message
        })
        {
            StatusCode = (int)HttpStatusCode.NotFound
        }
    },
    {
        typeof(InvalidExecutionAdapter),
        ex => new ObjectResult(new ErrorResponse
        {
            InnerCode = 3,
            Message = ex.Message
        })
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        }
    },
    {
        typeof(InUseValueAdapter),
        ex => new ObjectResult(new ErrorResponse
        {
            InnerCode = 4,
            Message = ex.Message
        })
        {
            StatusCode = (int)HttpStatusCode.Conflict
        }
    }
};

    public void OnException(ExceptionContext context)
    {
        var factory = _errorFactories.GetValueOrDefault(context.Exception.GetType());

        if(factory == null)
        {
            context.Result = new ObjectResult(new ErrorResponse
            {
                InnerCode = 6,
                Message = "There was an error when processing the request"
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            return;
        }

        context.Result = factory(context.Exception);
    }
}

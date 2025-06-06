using System.Net;
using IAdapter.Exceptions;
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
    },
        {
        typeof(Exception),
        ex => new ObjectResult(new ErrorResponse
        {
            InnerCode = 5,
            Message = ex.Message
        })
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        }
    },
};

    public void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        var factory = _errorFactories
            .Where(kvp => kvp.Key.IsAssignableFrom(exceptionType))
            .Select(kvp => kvp.Value)
            .FirstOrDefault();

        if(factory == null)
        {
            context.Result = new ObjectResult(new ErrorResponse
            {
                InnerCode = 6,
                Message = context.Exception.Message
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            return;
        }

        context.Result = factory(context.Exception);
    }
}

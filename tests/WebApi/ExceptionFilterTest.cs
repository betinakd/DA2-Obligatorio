using System.Net;
using Adapter.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using WebApi.Filters;

namespace Tests.WebApi;

[TestClass]
public class ExceptionFilterTests
{
    private ExceptionFilter? _exceptionFilter;
    private ExceptionContext CreateExceptionContext(Exception exception)
    {
        return new ExceptionContext(
            new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };
    }

    [TestInitialize]
    public void Setup()
    {
        _exceptionFilter = new ExceptionFilter();
    }

    [TestMethod]
    public void OnException_InvalidAttribute_ReturnsBadRequestResult()
    {
        var exceptionContext = CreateExceptionContext(new InvalidAttributeAdapter("Invalid attribute error"));

        _exceptionFilter!.OnException(exceptionContext);

        AssertResult(exceptionContext, HttpStatusCode.BadRequest, 1, "Invalid attribute error");
    }

    [TestMethod]
    public void OnException_ObjectNotFoundException_ReturnsNotFoundResult()
    {
        var exceptionContext = CreateExceptionContext(new NonExistentValueAdapter("Object not found"));

        _exceptionFilter!.OnException(exceptionContext);

        AssertResult(exceptionContext, HttpStatusCode.NotFound, 2, "Object not found");
    }

    [TestMethod]
    public void OnException_InvalidExecutionException_ReturnsBadRequestResult()
    {
        var exceptionContext = CreateExceptionContext(new InvalidExecutionAdapter("Invalid execution"));

        _exceptionFilter!.OnException(exceptionContext);

        AssertResult(exceptionContext, HttpStatusCode.BadRequest, 3, "Invalid execution");
    }

    private void AssertResult(ExceptionContext context, HttpStatusCode expectedStatusCode, int expectedInnerCode, string expectedMessage)
    {
        Assert.IsNotNull(context.Result);
        var objectResult = context.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)expectedStatusCode, objectResult!.StatusCode);

        var resultValue = objectResult.Value as ErrorResponse;
        Assert.IsNotNull(resultValue);
        Assert.AreEqual(expectedInnerCode, resultValue!.InnerCode);
        Assert.AreEqual(expectedMessage, resultValue.Message);
    }

    [TestMethod]
    public void OnException_GenericException_ReturnsInternalServerError()
    {
        var exceptionContext = CreateExceptionContext(new Exception("Generic error occurred"));

        _exceptionFilter!.OnException(exceptionContext);

        AssertResult(exceptionContext, HttpStatusCode.InternalServerError, 5, "Generic error occurred");
    }
}

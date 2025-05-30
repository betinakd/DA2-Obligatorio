using System.Net;
using IAdapter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using WebApi.Filters;

namespace Tests.WebApi;

[TestClass]
public class AuthorizationFilterTest
{
    private Mock<IExecutionAdapter>? _mockExecutionAdapter;
    private AuthorizationFilter? _authorizationFilter;

    [TestInitialize]
    public void Setup()
    {
        _mockExecutionAdapter = new Mock<IExecutionAdapter>();
        _authorizationFilter = new AuthorizationFilter(_mockExecutionAdapter.Object);
    }

    private AuthorizationFilterContext CreateContext(IHeaderDictionary headers)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Clear();

        foreach(var header in headers)
        {
            httpContext.Request.Headers[header.Key] = header.Value;
        }

        return new AuthorizationFilterContext(
            new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            ),
            []
        );
    }

    [TestMethod]
    public void OnAuthorization_MissingApiKey_ReturnsUnauthorized()
    {
        var context = CreateContext(new HeaderDictionary());

        _authorizationFilter.OnAuthorization(context);

        AssertUnauthorizedResult(context, "Invalid or missing API key");
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(It.IsAny<Guid>()), Times.Never);
    }

    private void AssertUnauthorizedResult(AuthorizationFilterContext context, string expectedMessage)
    {
        Assert.IsNotNull(context.Result);
        var objectResult = context.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, objectResult.StatusCode);

        var errorResponse = objectResult.Value as ErrorResponse;
        Assert.IsNotNull(errorResponse);
        Assert.AreEqual(7, errorResponse.InnerCode);
        Assert.AreEqual(expectedMessage, errorResponse.Message);
    }
}
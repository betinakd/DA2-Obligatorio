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
                new ActionDescriptor()),
            []);
    }

    [TestMethod]
    public void OnAuthorization_MissingApiKey_ReturnsUnauthorized()
    {
        var context = CreateContext(new HeaderDictionary());

        _authorizationFilter.OnAuthorization(context);

        AssertUnauthorizedResult(context, "Missing API key");
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void OnAuthorization_InvalidApiKeyFormat_ReturnsUnauthorized()
    {
        var headers = new HeaderDictionary
        {
            { "authorization", "not-a-valid-guid" },
        };
        var context = CreateContext(headers);

        _authorizationFilter.OnAuthorization(context);

        AssertUnauthorizedResult(context, "Invalid API key");
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void OnAuthorization_UnauthorizedApiKey_ReturnsUnauthorized()
    {
        var apiKey = Guid.NewGuid();
        var headers = new HeaderDictionary
        {
            { "authorization", apiKey.ToString() },
        };
        var context = CreateContext(headers);

        _mockExecutionAdapter.Setup(x => x.IsAuthorizedUser(apiKey)).Returns(false);

        _authorizationFilter.OnAuthorization(context);

        AssertUnauthorizedResult(context, "Invalid or missing API key");
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(apiKey), Times.Once);
    }

    [TestMethod]
    public void OnAuthorization_ValidApiKey_Succeeds()
    {
        var apiKey = Guid.Parse("77777777-aaaa-1111-1111-111111111111");
        var headers = new HeaderDictionary
        {
            { "authorization", apiKey.ToString() },
        };
        var context = CreateContext(headers);

        _mockExecutionAdapter.Setup(x => x.IsAuthorizedUser(apiKey)).Returns(true);

        _authorizationFilter.OnAuthorization(context);

        Assert.IsNull(context.Result);
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(apiKey), Times.Once);
    }

    [TestMethod]
    public void OnAuthorization_EmptyApiKey_ReturnsUnauthorized()
    {
        var headers = new HeaderDictionary
        {
            { "authorization", Guid.Empty.ToString() },
        };
        var context = CreateContext(headers);

        _mockExecutionAdapter.Setup(x => x.IsAuthorizedUser(Guid.Empty)).Returns(false);

        _authorizationFilter.OnAuthorization(context);

        AssertUnauthorizedResult(context, "Invalid or missing API key");
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(Guid.Empty), Times.Once);
    }

    [TestMethod]
    public void OnAuthorization_ApiKeyFromRequirements_Succeeds()
    {
        var apiKey = Guid.Parse("9C0FF0B1-4ABD-45C6-8A4A-831748FB7A20");
        var headers = new HeaderDictionary
        {
            { "authorization", apiKey.ToString() },
        };
        var context = CreateContext(headers);

        _mockExecutionAdapter.Setup(x => x.IsAuthorizedUser(apiKey)).Returns(true);

        _authorizationFilter.OnAuthorization(context);

        Assert.IsNull(context.Result);
        _mockExecutionAdapter.Verify(x => x.IsAuthorizedUser(apiKey), Times.Once);
    }

    private void AssertUnauthorizedResult(AuthorizationFilterContext context, string expectedMessage)
    {
        Assert.IsNotNull(context.Result);
        var objectResult = context.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, objectResult.StatusCode);

        var errorResponse = objectResult.Value as ErrorResponse;
        Assert.IsNotNull(errorResponse);
        Assert.AreEqual(8, errorResponse.InnerCode);
        Assert.AreEqual(expectedMessage, errorResponse.Message);
    }
}

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tests.WebApi;

[TestClass]
public class ModelStateValidationFilterTest
{
    [TestMethod]
    public void OnActionExecuting_InvalidModelState_ReturnsBadRequestWithCustomMessage()
    {
        var filter = new ModelStateValidationFilter();
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("State", "State is required and their values should be: Normal, Abstract, Sealed.");

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            modelState
        );

        var context = new ActionExecutingContext(
            actionContext,
            [],
            new Dictionary<string, object>(),
            null
        );

        filter.OnActionExecuting(context);

        Assert.IsInstanceOfType(context.Result, typeof(BadRequestObjectResult));
        var badRequest = context.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);

        var json = JsonSerializer.Serialize(badRequest.Value);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.AreEqual(5, root.GetProperty("innerCode").GetInt32());
        Assert.AreEqual("State is required and their values should be: Normal, Abstract, Sealed.", root.GetProperty("message").GetString());
    }
}
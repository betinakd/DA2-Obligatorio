using System.Net;
using IBusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class AuthorizationFilter(IApikeyService apikeyService) : IAuthorizationFilter
{
    private readonly IApikeyService _apiKeyService = apikeyService;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var apiKeyHeader))
        {
            SetUnauthorizedResult(context, "Missing API key");
            return;
        }

        if(!Guid.TryParse(apiKeyHeader, out var apiKeyValue))
        {
            SetUnauthorizedResult(context, "Invalid API key");
            return;
        }

        var isValidApiKey = _apiKeyService.IsAuthorizedUser(apiKeyValue);
        if(!isValidApiKey)
        {
            SetUnauthorizedResult(context, "Invalid or missing API key");
            return;
        }
    }

    private static void SetUnauthorizedResult(AuthorizationFilterContext context, string message)
    {
        context.Result = new ObjectResult(new ErrorResponse
        {
            InnerCode = 8,
            Message = message
        })
        {
            StatusCode = (int)HttpStatusCode.Unauthorized
        };
    }
}

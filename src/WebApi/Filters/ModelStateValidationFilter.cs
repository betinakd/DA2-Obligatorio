using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ModelStateValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if(!context.ModelState.IsValid)
        {
            var firstError = context.ModelState
                .SelectMany(x => x.Value.Errors)
                .FirstOrDefault();

            if(firstError != null)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    innerCode = 5,
                    message = firstError.ErrorMessage,
                });
                return;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using WebApi.Filters;

namespace WebApi.Controllers;

[ExceptionFilter]
[ApiController]
[Route("api/v1/executions")]
public class MethodExecutionController(IExecutionAdapter methodAdapter) : ControllerBase
{
    private readonly IExecutionAdapter _simMethodAdapter = methodAdapter;

    [HttpPost]
    public IActionResult ExecuteMethod([FromBody] MethodExecutionRequest request)
    {
        return Ok(_simMethodAdapter.ExecuteMethod(request));
    }
}

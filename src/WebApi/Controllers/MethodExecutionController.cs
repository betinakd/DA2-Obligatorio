using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

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

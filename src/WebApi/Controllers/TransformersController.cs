using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/transformers")]
public class TransformersController(ITransformerAdapter transformerService, IExecutionAdapter executionAdapter) : ControllerBase
{
    private readonly ITransformerAdapter _transformerService = transformerService;
    private readonly IExecutionAdapter _executionAdapter = executionAdapter;

    [HttpGet]
    public IActionResult GetTransformers()
    {
        return Ok(_transformerService.GetTransformers());
    }

    [HttpPost]
    public IActionResult ExecuteWithTransform(
        [FromBody] MethodExecutionRequest request,
        [FromQuery] string transformerId)
    {
        var result = _executionAdapter.ExecuteMethodWithTransform(request, transformerId);
        return Ok(result);
    }
}

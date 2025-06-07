using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/transformers")]
public class TransformersController(ITransformerAdapter transformerAdaptercopy) : ControllerBase
{
    private readonly ITransformerAdapter _transformerAdaptercopy = transformerAdaptercopy;

    [HttpGet]
    public IActionResult GetTransformers()
    {
        return Ok(_transformerAdaptercopy.GetTransformers());
    }

    [HttpPost]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public IActionResult ExecuteWithTransform([FromBody] MethodExecutionTransformedRequest request)
    {
        var result = _transformerAdaptercopy.ExportExecution(request);
        return Ok(result);
    }
}

using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class ClassMethodController(IMethodAdapter methodAdapter) : ControllerBase
{
    private readonly IMethodAdapter _simMethodAdapter = methodAdapter;

    [HttpPost("{id}/methods")]
    public IActionResult CreateMethod(Guid id, [FromBody] MethodRequest methodRequest)
    {
        var response = _simMethodAdapter.CreateMethod(id, methodRequest);
        return CreatedAtRoute("GetMethodId", new { id = response.MethodResponse.Id }, response);
    }
}

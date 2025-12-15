using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/project-evaluations")]
public class ProjectEvaluationController(IProjectEvaluationAdapter projectEvaluationAdapter) : ControllerBase
{
    private readonly IProjectEvaluationAdapter _projectEvaluationAdapter = projectEvaluationAdapter;

    [HttpPost]
    public IActionResult CreateProjectEvaluation([FromBody] ProjectEvaluationRequest request)
    {
        var response = _projectEvaluationAdapter.CreateProjectEvaluation(request);
        return CreatedAtRoute("GetProjectEvaluationById", new { id = response.Id }, response);
    }

    [HttpGet("{id}", Name = "GetProjectEvaluationById")]
    public IActionResult GetProjectEvaluationById(Guid id)
    {
        var response = _projectEvaluationAdapter.GetProjectEvaluationById(id);
        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetAllProjectEvaluations()
    {
        var evaluations = _projectEvaluationAdapter.GetAllProjectEvaluations();
        return Ok(evaluations);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProjectEvaluation(Guid id)
    {
        _projectEvaluationAdapter.DeleteProjectEvaluation(id);
        return NoContent();
    }
}

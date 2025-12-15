using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IProjectEvaluationAdapter
{
    ProjectEvaluationResponse CreateProjectEvaluation(ProjectEvaluationRequest request);
    ProjectEvaluationResponse GetProjectEvaluationById(Guid id);
    IEnumerable<ProjectEvaluationResponse> GetAllProjectEvaluations();
    void DeleteProjectEvaluation(Guid id);
}

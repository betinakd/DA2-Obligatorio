using Domain;

namespace IDataAccess;

public interface IProjectEvaluationDataAccess
{
    ProjectEvaluation Add(ProjectEvaluation projectEvaluation);
    ProjectEvaluation GetById(Guid id);
    IEnumerable<ProjectEvaluation> GetAll();
    void Delete(Guid id);
    bool Exists(Guid id);
}

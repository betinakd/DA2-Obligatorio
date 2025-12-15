using Domain;
using IDataAccess;

namespace DataAccess;

public class ProjectEvaluationDataAccess(SimulatorDbContext context) : IProjectEvaluationDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public ProjectEvaluation Add(ProjectEvaluation projectEvaluation)
    {
        _context.ProjectEvaluations.Add(projectEvaluation);
        _context.SaveChanges();
        return projectEvaluation;
    }

    public ProjectEvaluation GetById(Guid id)
    {
        var projectEvaluation = _context.ProjectEvaluations.Find(id);
        if (projectEvaluation == null)
        {
            throw new InvalidOperationException($"ProjectEvaluation with ID {id} not found");
        }

        return projectEvaluation;
    }

    public IEnumerable<ProjectEvaluation> GetAll()
    {
        return _context.ProjectEvaluations.ToList();
    }

    public void Delete(Guid id)
    {
        var projectEvaluation = _context.ProjectEvaluations.Find(id);
        if (projectEvaluation != null)
        {
            _context.ProjectEvaluations.Remove(projectEvaluation);
            _context.SaveChanges();
        }
    }

    public bool Exists(Guid id)
    {
        return _context.ProjectEvaluations.Any(pe => pe.Id == id);
    }
}

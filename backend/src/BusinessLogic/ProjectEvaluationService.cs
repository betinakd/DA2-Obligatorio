using Domain;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using IDataAccess;

namespace BusinessLogic;

public class ProjectEvaluationService(IProjectEvaluationDataAccess projectEvaluationDataAccess)
    : IProjectEvaluationService
{
    private readonly IProjectEvaluationDataAccess _projectEvaluationDA = projectEvaluationDataAccess;

    public ProjectEvaluation CreateProjectEvaluation(
        string projectName,
        decimal cac,
        decimal averageRevenuePerCustomer,
        decimal averagePurchaseFrequency,
        decimal customerLifespan,
        decimal grossMargin,
        decimal churnRate,
        bool includeTaxesInCLV,
        decimal taxRate)
    {
        var projectEvaluation = new ProjectEvaluation
        {
            Id = Guid.NewGuid(),
            ProjectName = projectName,
            CustomerAcquisitionCost = cac,
            AverageRevenuePerCustomer = averageRevenuePerCustomer,
            AveragePurchaseFrequency = averagePurchaseFrequency,
            CustomerLifespan = customerLifespan,
            GrossMargin = grossMargin,
            ChurnRate = churnRate,
            IncludeTaxesInCLV = includeTaxesInCLV,
            TaxRate = taxRate,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            projectEvaluation.Validate();
        }
        catch (ArgumentException ex)
        {
            throw new InvalidAttributeLogic(ex.Message);
        }

        return _projectEvaluationDA.Add(projectEvaluation);
    }

    public ProjectEvaluation GetProjectEvaluationById(Guid id)
    {
        if (!_projectEvaluationDA.Exists(id))
        {
            throw new NonExistentValueLogic("Project evaluation not found");
        }

        return _projectEvaluationDA.GetById(id);
    }

    public IEnumerable<ProjectEvaluation> GetAllProjectEvaluations()
    {
        return _projectEvaluationDA.GetAll();
    }

    public void DeleteProjectEvaluation(Guid id)
    {
        if (!_projectEvaluationDA.Exists(id))
        {
            throw new NonExistentValueLogic("Project evaluation not found");
        }

        _projectEvaluationDA.Delete(id);
    }
}

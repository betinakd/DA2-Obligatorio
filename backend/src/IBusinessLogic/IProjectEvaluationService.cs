using Domain;

namespace IBusinessLogic;

public interface IProjectEvaluationService
{
    ProjectEvaluation CreateProjectEvaluation(
        string projectName,
        decimal cac,
        decimal averageRevenuePerCustomer,
        decimal averagePurchaseFrequency,
        decimal customerLifespan,
        decimal grossMargin,
        decimal churnRate,
        bool includeTaxesInCLV,
        decimal taxRate);

    ProjectEvaluation GetProjectEvaluationById(Guid id);

    IEnumerable<ProjectEvaluation> GetAllProjectEvaluations();

    void DeleteProjectEvaluation(Guid id);
}

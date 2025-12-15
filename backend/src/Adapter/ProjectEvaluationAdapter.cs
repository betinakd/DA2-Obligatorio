using Domain;
using IAdapter;
using IAdapter.Exceptions;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using Models.Request;
using Models.Response;

namespace Adapter;

public class ProjectEvaluationAdapter(IProjectEvaluationService projectEvaluationService)
    : IProjectEvaluationAdapter
{
    private readonly IProjectEvaluationService _projectEvaluationService = projectEvaluationService;

    public ProjectEvaluationResponse CreateProjectEvaluation(ProjectEvaluationRequest request)
    {
        try
        {
            var projectEvaluation = _projectEvaluationService.CreateProjectEvaluation(
                request.ProjectName,
                request.CustomerAcquisitionCost,
                request.AverageRevenuePerCustomer,
                request.AveragePurchaseFrequency,
                request.CustomerLifespan,
                request.GrossMargin,
                request.ChurnRate,
                request.IncludeTaxesInCLV,
                request.TaxRate);

            return MapToResponse(projectEvaluation);
        }
        catch (InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
    }

    public ProjectEvaluationResponse GetProjectEvaluationById(Guid id)
    {
        try
        {
            var projectEvaluation = _projectEvaluationService.GetProjectEvaluationById(id);
            return MapToResponse(projectEvaluation);
        }
        catch (NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    public IEnumerable<ProjectEvaluationResponse> GetAllProjectEvaluations()
    {
        var projectEvaluations = _projectEvaluationService.GetAllProjectEvaluations();
        return projectEvaluations.Select(MapToResponse);
    }

    public void DeleteProjectEvaluation(Guid id)
    {
        try
        {
            _projectEvaluationService.DeleteProjectEvaluation(id);
        }
        catch (NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    private static ProjectEvaluationResponse MapToResponse(ProjectEvaluation projectEvaluation)
    {
        return new ProjectEvaluationResponse
        {
            Id = projectEvaluation.Id,
            ProjectName = projectEvaluation.ProjectName,
            CustomerAcquisitionCost = projectEvaluation.CustomerAcquisitionCost,
            CustomerLifetimeValue = projectEvaluation.CalculateCustomerLifetimeValue(),
            ReturnOnInvestment = projectEvaluation.CalculateReturnOnInvestment(),
            CLVtoCACRatio = projectEvaluation.CalculateCLVtoCAC(),
            ChurnRate = projectEvaluation.ChurnRate,
            IncludedTaxesInCLV = projectEvaluation.IncludeTaxesInCLV,
            CreatedAt = projectEvaluation.CreatedAt
        };
    }
}

using IAdapter;
using IAdapter.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class ProjectEvaluationControllerTest
{
    private Mock<IProjectEvaluationAdapter>? _mockAdapter;
    private ProjectEvaluationController? _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockAdapter = new Mock<IProjectEvaluationAdapter>();
        _controller = new ProjectEvaluationController(_mockAdapter.Object);
    }

    [TestMethod]
    public void CreateProjectEvaluation_ValidRequest_ReturnsCreatedResult()
    {
        var request = new ProjectEvaluationRequest
        {
            ProjectName = "Test Project",
            CustomerAcquisitionCost = 100m,
            AverageRevenuePerCustomer = 200m,
            AveragePurchaseFrequency = 3m,
            CustomerLifespan = 4m,
            GrossMargin = 0.5m,
            ChurnRate = 0.1m,
            IncludeTaxesInCLV = false,
            TaxRate = 0m
        };

        var response = new ProjectEvaluationResponse
        {
            Id = Guid.NewGuid(),
            ProjectName = request.ProjectName,
            CustomerAcquisitionCost = request.CustomerAcquisitionCost,
            CustomerLifetimeValue = 1200m,
            ReturnOnInvestment = 11m,
            CLVtoCACRatio = 12m,
            ChurnRate = request.ChurnRate,
            IncludedTaxesInCLV = request.IncludeTaxesInCLV,
            CreatedAt = DateTime.UtcNow
        };

        _mockAdapter!
            .Setup(m => m.CreateProjectEvaluation(request))
            .Returns(response);

        var result = _controller!.CreateProjectEvaluation(request);

        Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        var createdResult = result as CreatedAtRouteResult;
        Assert.AreEqual("GetProjectEvaluationById", createdResult!.RouteName);
        Assert.AreEqual(response, createdResult.Value);
    }

    [TestMethod]
    public void GetProjectEvaluationById_ExistingId_ReturnsOkResult()
    {
        var id = Guid.NewGuid();
        var response = new ProjectEvaluationResponse
        {
            Id = id,
            ProjectName = "Test Project",
            CustomerAcquisitionCost = 100m,
            CustomerLifetimeValue = 1200m,
            ReturnOnInvestment = 11m,
            CLVtoCACRatio = 12m
        };

        _mockAdapter!
            .Setup(m => m.GetProjectEvaluationById(id))
            .Returns(response);

        var result = _controller!.GetProjectEvaluationById(id);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(response, okResult!.Value);
    }

    [TestMethod]
    public void GetProjectEvaluationById_NonExistingId_ThrowsException()
    {
        var id = Guid.NewGuid();

        _mockAdapter!
            .Setup(m => m.GetProjectEvaluationById(id))
            .Throws(new NonExistentValueAdapter("Not found"));

        Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _controller!.GetProjectEvaluationById(id));
    }

    [TestMethod]
    public void GetAllProjectEvaluations_ReturnsOkResult()
    {
        var responses = new List<ProjectEvaluationResponse>
        {
            new ProjectEvaluationResponse
            {
                Id = Guid.NewGuid(),
                ProjectName = "Project 1",
                CustomerAcquisitionCost = 100m,
                CustomerLifetimeValue = 1200m
            },
            new ProjectEvaluationResponse
            {
                Id = Guid.NewGuid(),
                ProjectName = "Project 2",
                CustomerAcquisitionCost = 150m,
                CustomerLifetimeValue = 1500m
            }
        };

        _mockAdapter!
            .Setup(m => m.GetAllProjectEvaluations())
            .Returns(responses);

        var result = _controller!.GetAllProjectEvaluations();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(responses, okResult!.Value);
    }

    [TestMethod]
    public void DeleteProjectEvaluation_ExistingId_ReturnsNoContentResult()
    {
        var id = Guid.NewGuid();

        _mockAdapter!
            .Setup(m => m.DeleteProjectEvaluation(id));

        var result = _controller!.DeleteProjectEvaluation(id);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _mockAdapter.Verify(m => m.DeleteProjectEvaluation(id), Times.Once);
    }

    [TestMethod]
    public void DeleteProjectEvaluation_NonExistingId_ThrowsException()
    {
        var id = Guid.NewGuid();

        _mockAdapter!
            .Setup(m => m.DeleteProjectEvaluation(id))
            .Throws(new NonExistentValueAdapter("Not found"));

        Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _controller!.DeleteProjectEvaluation(id));
    }
}

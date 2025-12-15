using Adapter;
using Domain;
using IAdapter.Exceptions;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using Models.Request;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class ProjectEvaluationAdapterTest
{
    private Mock<IProjectEvaluationService>? _mockService;
    private ProjectEvaluationAdapter? _adapter;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IProjectEvaluationService>();
        _adapter = new ProjectEvaluationAdapter(_mockService.Object);
    }

    [TestMethod]
    public void CreateProjectEvaluation_ValidRequest_ReturnsResponse()
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

        var projectEvaluation = new ProjectEvaluation
        {
            Id = Guid.NewGuid(),
            ProjectName = request.ProjectName,
            CustomerAcquisitionCost = request.CustomerAcquisitionCost,
            AverageRevenuePerCustomer = request.AverageRevenuePerCustomer,
            AveragePurchaseFrequency = request.AveragePurchaseFrequency,
            CustomerLifespan = request.CustomerLifespan,
            GrossMargin = request.GrossMargin,
            ChurnRate = request.ChurnRate,
            IncludeTaxesInCLV = request.IncludeTaxesInCLV,
            TaxRate = request.TaxRate,
            CreatedAt = DateTime.UtcNow
        };

        _mockService!
            .Setup(m => m.CreateProjectEvaluation(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<bool>(),
                It.IsAny<decimal>()))
            .Returns(projectEvaluation);

        var response = _adapter!.CreateProjectEvaluation(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(projectEvaluation.Id, response.Id);
        Assert.AreEqual(projectEvaluation.ProjectName, response.ProjectName);
    }

    [TestMethod]
    public void CreateProjectEvaluation_InvalidData_ThrowsException()
    {
        var request = new ProjectEvaluationRequest
        {
            ProjectName = "Test Project",
            CustomerAcquisitionCost = -100m
        };

        _mockService!
            .Setup(m => m.CreateProjectEvaluation(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                It.IsAny<bool>(),
                It.IsAny<decimal>()))
            .Throws(new InvalidAttributeLogic("Invalid CAC"));

        Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _adapter!.CreateProjectEvaluation(request));
    }

    [TestMethod]
    public void GetProjectEvaluationById_ExistingId_ReturnsResponse()
    {
        var id = Guid.NewGuid();
        var projectEvaluation = new ProjectEvaluation
        {
            Id = id,
            ProjectName = "Test Project",
            CustomerAcquisitionCost = 100m,
            AverageRevenuePerCustomer = 200m,
            AveragePurchaseFrequency = 3m,
            CustomerLifespan = 4m,
            GrossMargin = 0.5m
        };

        _mockService!
            .Setup(m => m.GetProjectEvaluationById(id))
            .Returns(projectEvaluation);

        var response = _adapter!.GetProjectEvaluationById(id);

        Assert.IsNotNull(response);
        Assert.AreEqual(id, response.Id);
        Assert.AreEqual("Test Project", response.ProjectName);
    }

    [TestMethod]
    public void GetProjectEvaluationById_NonExistingId_ThrowsException()
    {
        var id = Guid.NewGuid();

        _mockService!
            .Setup(m => m.GetProjectEvaluationById(id))
            .Throws(new NonExistentValueLogic("Not found"));

        Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _adapter!.GetProjectEvaluationById(id));
    }

    [TestMethod]
    public void GetAllProjectEvaluations_ReturnsListOfResponses()
    {
        var evaluations = new List<ProjectEvaluation>
        {
            new ProjectEvaluation
            {
                Id = Guid.NewGuid(),
                ProjectName = "Project 1",
                CustomerAcquisitionCost = 100m,
                AverageRevenuePerCustomer = 200m,
                AveragePurchaseFrequency = 3m,
                CustomerLifespan = 4m,
                GrossMargin = 0.5m
            },
            new ProjectEvaluation
            {
                Id = Guid.NewGuid(),
                ProjectName = "Project 2",
                CustomerAcquisitionCost = 150m,
                AverageRevenuePerCustomer = 250m,
                AveragePurchaseFrequency = 2m,
                CustomerLifespan = 5m,
                GrossMargin = 0.6m
            }
        };

        _mockService!
            .Setup(m => m.GetAllProjectEvaluations())
            .Returns(evaluations);

        var responses = _adapter!.GetAllProjectEvaluations();

        Assert.AreEqual(2, responses.Count());
    }

    [TestMethod]
    public void DeleteProjectEvaluation_ExistingId_DeletesSuccessfully()
    {
        var id = Guid.NewGuid();

        _mockService!
            .Setup(m => m.DeleteProjectEvaluation(id));

        _adapter!.DeleteProjectEvaluation(id);

        _mockService.Verify(m => m.DeleteProjectEvaluation(id), Times.Once);
    }

    [TestMethod]
    public void DeleteProjectEvaluation_NonExistingId_ThrowsException()
    {
        var id = Guid.NewGuid();

        _mockService!
            .Setup(m => m.DeleteProjectEvaluation(id))
            .Throws(new NonExistentValueLogic("Not found"));

        Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _adapter!.DeleteProjectEvaluation(id));
    }
}

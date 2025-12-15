using BusinessLogic;
using Domain;
using IBusinessLogic.Exceptions;
using IDataAccess;
using Moq;

namespace Tests.BusinessLogic;

[TestClass]
public class ProjectEvaluationServiceTest
{
    private Mock<IProjectEvaluationDataAccess>? _mockDataAccess;
    private ProjectEvaluationService? _service;

    [TestInitialize]
    public void Initialize()
    {
        _mockDataAccess = new Mock<IProjectEvaluationDataAccess>(MockBehavior.Strict);
        _service = new ProjectEvaluationService(_mockDataAccess.Object);
    }

    [TestMethod]
    public void CreateProjectEvaluation_ValidInputs_ReturnsProjectEvaluation()
    {
        var expectedProjectEvaluation = new ProjectEvaluation
        {
            Id = Guid.NewGuid(),
            ProjectName = "Test Project",
            CustomerAcquisitionCost = 100m,
            AverageRevenuePerCustomer = 200m,
            AveragePurchaseFrequency = 3m,
            CustomerLifespan = 4m,
            GrossMargin = 0.5m,
            ChurnRate = 0.1m,
            IncludeTaxesInCLV = false,
            TaxRate = 0m,
            CreatedAt = DateTime.UtcNow
        };

        _mockDataAccess!
            .Setup(m => m.Add(It.IsAny<ProjectEvaluation>()))
            .Returns(expectedProjectEvaluation);

        var result = _service!.CreateProjectEvaluation(
            "Test Project",
            100m,
            200m,
            3m,
            4m,
            0.5m,
            0.1m,
            false,
            0m);

        Assert.IsNotNull(result);
        _mockDataAccess.Verify(m => m.Add(It.IsAny<ProjectEvaluation>()), Times.Once);
    }

    [TestMethod]
    public void CreateProjectEvaluation_InvalidInputs_ThrowsException()
    {
        Assert.ThrowsException<InvalidAttributeLogic>(() =>
            _service!.CreateProjectEvaluation(
                "Test Project",
                -100m,
                200m,
                3m,
                4m,
                0.5m,
                0.1m,
                false,
                0m));
    }

    [TestMethod]
    public void GetProjectEvaluationById_ExistingId_ReturnsProjectEvaluation()
    {
        var id = Guid.NewGuid();
        var expectedProjectEvaluation = new ProjectEvaluation
        {
            Id = id,
            ProjectName = "Test Project",
            CustomerAcquisitionCost = 100m
        };

        _mockDataAccess!
            .Setup(m => m.Exists(id))
            .Returns(true);

        _mockDataAccess
            .Setup(m => m.GetById(id))
            .Returns(expectedProjectEvaluation);

        var result = _service!.GetProjectEvaluationById(id);

        Assert.AreEqual(id, result.Id);
        _mockDataAccess.Verify(m => m.Exists(id), Times.Once);
        _mockDataAccess.Verify(m => m.GetById(id), Times.Once);
    }

    [TestMethod]
    public void GetProjectEvaluationById_NonExistingId_ThrowsException()
    {
        var id = Guid.NewGuid();

        _mockDataAccess!
            .Setup(m => m.Exists(id))
            .Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _service!.GetProjectEvaluationById(id));

        _mockDataAccess.Verify(m => m.Exists(id), Times.Once);
    }

    [TestMethod]
    public void GetAllProjectEvaluations_ReturnsListOfEvaluations()
    {
        var expectedList = new List<ProjectEvaluation>
        {
            new ProjectEvaluation { Id = Guid.NewGuid(), ProjectName = "Project 1" },
            new ProjectEvaluation { Id = Guid.NewGuid(), ProjectName = "Project 2" }
        };

        _mockDataAccess!
            .Setup(m => m.GetAll())
            .Returns(expectedList);

        var result = _service!.GetAllProjectEvaluations();

        Assert.AreEqual(2, result.Count());
        _mockDataAccess.Verify(m => m.GetAll(), Times.Once);
    }

    [TestMethod]
    public void DeleteProjectEvaluation_ExistingId_DeletesSuccessfully()
    {
        var id = Guid.NewGuid();

        _mockDataAccess!
            .Setup(m => m.Exists(id))
            .Returns(true);

        _mockDataAccess
            .Setup(m => m.Delete(id));

        _service!.DeleteProjectEvaluation(id);

        _mockDataAccess.Verify(m => m.Exists(id), Times.Once);
        _mockDataAccess.Verify(m => m.Delete(id), Times.Once);
    }

    [TestMethod]
    public void DeleteProjectEvaluation_NonExistingId_ThrowsException()
    {
        var id = Guid.NewGuid();

        _mockDataAccess!
            .Setup(m => m.Exists(id))
            .Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _service!.DeleteProjectEvaluation(id));

        _mockDataAccess.Verify(m => m.Exists(id), Times.Once);
    }
}

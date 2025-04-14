using Adapter;
using Domain;
using IBussinesLogic;
using Models.Request;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class SimClassAdapterTest
{
    private Mock<ISimClassService>? _mockSimClassService;
    private SimClassAdapter? _simClassAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object);
    }

    [TestMethod]
    public void GetAllSimClasses_ShouldReturnListOfSimClassResponses()
    {
        var simClasses = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "ClassA" },
            new SimClass { Id = Guid.NewGuid(), Name = "ClassB" }
        };

        _mockSimClassService
            ?.Setup(service => service.GetAllSimClasses())
            .Returns(simClasses);

        var result = _simClassAdapter?.GetAllSimClasses();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result!.Count);
        Assert.AreEqual(simClasses[0].Id, result[0].Id);
        Assert.AreEqual(simClasses[0].Name, result[0].Name);
        Assert.AreEqual(simClasses[1].Id, result[1].Id);
        Assert.AreEqual(simClasses[1].Name, result[1].Name);

        _mockSimClassService?.Verify(service => service.GetAllSimClasses(), Times.Once);
    }

    [TestMethod]
    public void CreateSimClass_ShouldReturnCreatedSimClassResponse_WhenValidRequest()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "ValidClass" };
        var request = new SimClassRequest
        {
            Name = "ValidClass",
            IsAbstract = false,
            IsSealed = false,
            BaseClassId = Guid.NewGuid()
        };

        _mockSimClassService
            ?.Setup(service => service.CreateSimClass(request.Name, request.IsAbstract, request.IsSealed, request.BaseClassId))
            .Returns(simClass);

        var result = _simClassAdapter?.CreateSimClass(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Class created successfully", result!.Message);
        Assert.AreEqual(simClass.Id, result.SimClass?.Id);
        Assert.AreEqual(simClass.Name, result.SimClass?.Name);

        _mockSimClassService?.Verify(service => service.CreateSimClass(request.Name, request.IsAbstract, request.IsSealed, request.BaseClassId), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClassCorrectly_ShouldReturnSimClassResponse()
    {
        // Arrange
        var simClassId = Guid.NewGuid();
        var request = new UpdateSimClassRequest
        {
            Id = simClassId,
            Name = "UpdatedClass"
        };

        var expectedSimClass = new SimClass { Id = simClassId, Name = "UpdatedClass" };

        _mockSimClassService
            ?.Setup(service => service.UpdateSimClass(It.Is<SimClass>(s => s.Id == request.Id && s.Name == request.Name)))
            .Returns(expectedSimClass);

        var result = _simClassAdapter?.UpdateSimClass(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedSimClass.Id, result?.Id);
        Assert.AreEqual(expectedSimClass.Name, result?.Name);

        _mockSimClassService?.Verify(service => service.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
    }
}

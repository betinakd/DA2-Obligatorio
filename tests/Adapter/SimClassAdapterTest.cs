using Adapter.Exceptions;
using Domain;
using Domain.Enums;
using IBussinesLogic;
using Models.Enums;
using Models.Request;
using Models.Response;
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
            BaseClassId = Guid.NewGuid(),
            State = SimModelsAccesibility.Normal
        };

        _mockSimClassService
            ?.Setup(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId))
            .Returns(simClass);

        var result = _simClassAdapter?.CreateSimClass(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Class created successfully", result!.Message);
        Assert.AreEqual(simClass.Id, result.SimClass?.Id);
        Assert.AreEqual(simClass.Name, result.SimClass?.Name);

        _mockSimClassService?.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClassCorrectly_ShouldReturnSimClassResponse()
    {
        var simClassId = Guid.NewGuid();
        var request = new UpdateSimClassRequest
        {
            Id = simClassId,
            Name = "UpdatedClass"
        };

        var expectedSimClass = new SimClass { Id = simClassId, Name = "UpdatedClass" };
        var expectedResult = new UpdateSimClassResponse
        {
            Message = "Class updated successfully",
            SimClass = new SimClassResponse()
            {
                Id = expectedSimClass.Id,
                Message = "Class updated Successfully",
                Name = expectedSimClass.Name,
                State = SimModelsAccesibility.Abstract
            }
        };

        _mockSimClassService
            ?.Setup(service => service.UpdateSimClass(It.Is<SimClass>(s => s.Id == request.Id && s.Name == request.Name)))
            .Returns(expectedSimClass);

        var result = _simClassAdapter?.UpdateSimClass(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult.Message, result?.Message);
        Assert.AreEqual(expectedSimClass.Id, result?.SimClass?.Id);
        Assert.AreEqual(expectedSimClass.Name, result?.SimClass?.Name);
        Assert.AreEqual(expectedSimClass.Name, result?.SimClass?.Name);

        _mockSimClassService?.Verify(service => service.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInvalidAttribute_WhenSimClassIsEmpty()
    {
        var request = new UpdateSimClassRequest() { Name = " " };

        var exception = Assert.ThrowsException<InvalidAttribute>(() =>
            _simClassAdapter?.UpdateSimClass(request));
        Assert.AreEqual("Name cannot be null or empty.", exception.Message);
    }

    [TestMethod]
    public void DeleteNonExistentClass_ShouldThrowObjectNotFoundException()
    {
        var simClassId = Guid.NewGuid();
        var simClasses = new List<SimClass>();

        _mockSimClassService
            ?.Setup(service => service.DeleteSimClass(simClassId))
            .Throws(new Exception());

        var exception = Assert.ThrowsException<ObjectNotFoundException>(() =>
            _simClassAdapter?.DeleteSimClass(simClassId));

        Assert.AreEqual($"Any class with the specified {simClassId} id exists.", exception.Message);

        _mockSimClassService?.Verify(service => service.DeleteSimClass(simClassId), Times.Once);
    }

    [TestMethod]
    public void GetNonExistentClass_ShouldThrowObjectNotFoundException()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass() { Id = simClassId, Name = "Name" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name, State = SimModelsAccesibility.Normal, Message = "Class not found" };
        _mockSimClassService
            ?.Setup(service => service.GetSimClassById(simClassId)).Throws(new Exception());

        var exception = Assert.ThrowsException<ObjectNotFoundException>(() =>
            _simClassAdapter?.GetSimClassInfo(simClassId));
        _mockSimClassService?.Verify(service => service.GetSimClassById(simClassId), Times.Once);
    }

    [TestMethod]
    public void GetExistentClass_ShouldReturnSimClassResponse()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "ExistingClass" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name, State = SimModelsAccesibility.Normal, Message = "Class found" };

        _mockSimClassService
            ?.Setup(service => service.GetSimClassById(simClassId))
            .Returns(simClass);

        var result = _simClassAdapter?.GetSimClassInfo(simClassId);

        Assert.IsNotNull(result);
        Assert.AreEqual(simClassResponse.Id, result?.Id);
        Assert.AreEqual(simClassResponse.Name, result?.Name);

        _mockSimClassService?.Verify(service => service.GetSimClassById(simClassId), Times.Once);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldCallService_WhenIdIsValid()
    {
        var simClassId = Guid.NewGuid();

        _mockSimClassService
            ?.Setup(service => service.DeleteSimClass(simClassId))
            .Verifiable();

        _simClassAdapter?.DeleteSimClass(simClassId);

        _mockSimClassService?.Verify(service => service.DeleteSimClass(simClassId), Times.Once);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldThrowObjectNotFoundException_WhenServiceThrowsException()
    {
        var simClassId = Guid.NewGuid();

        _mockSimClassService
            ?.Setup(service => service.DeleteSimClass(simClassId))
            .Throws(new Exception());

        var exception = Assert.ThrowsException<ObjectNotFoundException>(() =>
            _simClassAdapter?.DeleteSimClass(simClassId));

        Assert.AreEqual($"Any class with the specified {simClassId} id exists.", exception.Message);

        _mockSimClassService?.Verify(service => service.DeleteSimClass(simClassId), Times.Once);
    }
}

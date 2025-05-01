using Adapter;
using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
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
    private Mock<IExecutionService>? _mockExecutionService;
    private SimClassAdapter? _simClassAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockExecutionService.Object);
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
            IdBaseClass = Guid.NewGuid().ToString(),
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
    public void GetNonExistentClass_ShouldThrowNonExistentValueAdapter()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass() { Id = simClassId, Name = "Name" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name, State = SimModelsAccesibility.Normal };
        _mockSimClassService
            ?.Setup(service => service.GetSimClassById(simClassId)).Throws(new NonExistentValueLogic("Class not found."));

        var exception = Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _simClassAdapter?.GetSimClassInfo(simClassId));
        _mockSimClassService?.Verify(service => service.GetSimClassById(simClassId), Times.Once);
    }

    [TestMethod]
    public void GetExistentClass_ShouldReturnSimClassResponse()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "ExistingClass" };
        var simClassResponse = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name, State = SimModelsAccesibility.Normal };

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
    public void DeleteSimClass_ShouldThrowNonExistentValueAdapter_WhenServiceThrowsException()
    {
        var simClassId = Guid.NewGuid();

        _mockSimClassService
            ?.Setup(service => service.DeleteSimClass(simClassId))
            .Throws(new NonExistentValueLogic("Class does not exist."));

        var exception = Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _simClassAdapter?.DeleteSimClass(simClassId));

        Assert.AreEqual("Class does not exist.", exception.Message);

        _mockSimClassService?.Verify(service => service.DeleteSimClass(simClassId), Times.Once);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowInUseException_WhenServiceThrowsInUseValueLogic()
    {
        var request = new SimClassRequest
        {
            Name = "TestClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString()
        };

        _mockSimClassService!
            .Setup(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId))
            .Throws(new InUseValueLogic("Class is already in use."));

        var exception = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simClassAdapter!.CreateSimClass(request));

        Assert.AreEqual("Class is already in use.", exception.Message);

        _mockSimClassService.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId), Times.Once);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldThrowInUseValueAdapter_WhenClassIsInUse()
    {
        var simClassId = Guid.NewGuid();

        _mockSimClassService
            ?.Setup(service => service.DeleteSimClass(simClassId))
            .Throws(new InUseValueLogic("Class is in use and cannot be deleted."));

        var exception = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simClassAdapter?.DeleteSimClass(simClassId));

        Assert.AreEqual("Class is in use and cannot be deleted.", exception.Message);

        _mockSimClassService?.Verify(service => service.DeleteSimClass(simClassId), Times.Once);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeLogic()
    {
        var request = new SimClassRequest
        {
            Name = "Invalid-Name-With-Chars",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString()
        };

        _mockSimClassService!
            .Setup(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId))
            .Throws(new InvalidAttributeLogic("Name contains invalid characters."));

        var exception = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simClassAdapter!.CreateSimClass(request));

        Assert.AreEqual("Name contains invalid characters.", exception.Message);

        _mockSimClassService.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateSimClass_ShouldThrowNonExistentValueAdapter_WhenServiceThrowsNonExistentValueLogic()
    {
        var request = new SimClassRequest
        {
            Name = "TestClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString()
        };

        _mockSimClassService!
            .Setup(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId))
            .Throws(new NonExistentValueLogic("Base class not found."));

        _simClassAdapter!.CreateSimClass(request);

        _mockSimClassService.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldReturnResponse_WhenValidData()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();
        var typeId = Guid.NewGuid();
        var returnTypeId = Guid.NewGuid();

        var request = new SimClassRequestCreateClass
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Attributes =
        [
            new()
            {
                Name = "TestAttribute",
                Privacity = SimModelsPrivacity.Public,
                IdClassType = typeId.ToString()
            }

        ],
            Methods =
        [
            new()
            {
                Name = "TestMethod",
                Accesibility = SimModelsAccesibility.Normal,
                Privacity = SimModelsPrivacity.Public,
                IdReturnType = returnTypeId.ToString(),
                Parameters =
                [
                    new()
                    {
                        Name = "param1",
                        IdClassType = typeId.ToString()
                    }

                ]
            }

        ]
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass" };
        var returnType = new SimClass { Id = returnTypeId, Name = "ReturnType" };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(returnTypeId))
            .Returns(returnType);

        _mockExecutionService!
            .Setup(s => s.MethodIsOverridingSealed(classId, It.IsAny<SimMethod>()))
            .Verifiable();

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Returns((SimClass sc) => sc);

        var result = _simClassAdapter!.UpdateSimClass(request, classId);

        Assert.IsNotNull(result);
        Assert.AreEqual(classId, result.Id);
        Assert.AreEqual("Class updated successfully", result.Message);

        _mockSimClassService.Verify(s => s.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(typeId), Times.Exactly(2)); // Una vez para atributo y otra para parámetro
        _mockSimClassService.Verify(s => s.GetSimClassById(returnTypeId), Times.Once);
        _mockExecutionService.Verify(s => s.MethodIsOverridingSealed(classId, It.IsAny<SimMethod>()), Times.Once);
        _mockSimClassService.Verify(s => s.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowNonExistentValueAdapter_WhenSimpleNonExistentValueLogic()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestCreateClass
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = []
        };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(baseClassId))
            .Throws(new NonExistentValueLogic("Base class not found"));

        var exception = Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _simClassAdapter!.UpdateSimClass(request, classId));

        Assert.AreEqual("Base class not found", exception.Message);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInUseValueAdapter_WhenSimpleInUseValueLogic()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestCreateClass
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = []
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Throws(new InUseValueLogic("Class in use"));

        var exception = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simClassAdapter!.UpdateSimClass(request, classId));

        Assert.AreEqual("Class in use", exception.Message);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInvalidAttributeAdapter_WhenSimpleInvalidAttributeDomain()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestCreateClass
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = []
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };

        _mockSimClassService!
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Throws(new InvalidAttributeDomain("Invalid attribute"));

        var exception = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simClassAdapter!.UpdateSimClass(request, classId));

        Assert.AreEqual("Invalid attribute", exception.Message);
    }
}

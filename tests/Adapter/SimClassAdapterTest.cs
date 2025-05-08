using Adapter;
using Adapter.Exceptions;
using BussinesLogic.Exceptions;
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
        var request = new SimClassRequestCreate
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
        var request = new SimClassRequestCreate
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
        var request = new SimClassRequestCreate
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
        var request = new SimClassRequestCreate
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
    public void UpdateSimClass_ShouldThrowNonExistentValueAdapter_WhenSimpleNonExistentValueLogic()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestUpdate
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
    public void UpdateSimClass_ShouldThrowNonExistentValueAdapter_WhenBaseClassNotFound()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = []
        };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockExecutionService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Throws(new NonExistentValueLogic("Base class not found"));

        var exception = Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _simClassAdapter.UpdateSimClass(request, classId));

        Assert.AreEqual("Base class not found", exception.Message);
        _mockSimClassService.Verify(s => s.GetSimClassById(baseClassId), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeDomain()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = []
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Sealed };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockExecutionService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        var exception = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simClassAdapter.UpdateSimClass(request, classId));

        Assert.AreEqual("Cannot set as base a sealed or null Class.", exception.Message);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInUseValueAdapter_WhenClassIsInUse()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = []
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Normal };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockExecutionService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Throws(new InUseValueLogic("Class is in use"));

        var exception = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simClassAdapter.UpdateSimClass(request, classId));

        Assert.AreEqual("Class is in use", exception.Message);
        _mockSimClassService.Verify(s => s.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldReturnSuccessResponse_WhenUpdateIsSuccessful()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();
        var objTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods =
        [
            new MethodRequest
            {
                Name = "TestMethod",
                Privacity = SimModelsPrivacity.Public,
                Accesibility = SimModelsAccesibility.Normal,
                IdReturnType = objTypeId.ToString(),
                Parameters =
                [
                    new ParameterRequest { Name = "param1", IdClassType = objTypeId.ToString() }
                ]
            }

        ],
            Attributes =
        [
            new AttributeRequest
            {
                Name = "TestAttribute",
                Privacity = SimModelsPrivacity.Private,
                IdClassType = objTypeId.ToString()
            }

        ]
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Normal };
        var objectClass = new SimClass { Id = objTypeId, Name = "Object" };
        var updatedClass = new SimClass { Id = classId, Name = "UpdatedClass", BaseClassId = baseClassId };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockExecutionService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(objTypeId))
            .Returns(objectClass);

        _mockExecutionService
            .Setup(s => s.MethodIsOverridingSealed(classId, It.IsAny<SimMethod>()));

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Callback<SimClass>(c => updatedClass = c)
            .Returns(updatedClass);

        var result = _simClassAdapter.UpdateSimClass(request, classId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Class updated successfully", result.Message);
        Assert.IsNotNull(result.SimClass);
        Assert.AreEqual(classId, result.SimClass.Id);
        Assert.AreEqual("UpdatedClass", result.SimClass.Name);

        _mockSimClassService.Verify(s => s.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
        _mockExecutionService.Verify(s => s.MethodIsOverridingSealed(classId, It.IsAny<SimMethod>()), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldSetDefaultObjectId_WhenBaseClassIdIsEmpty()
    {
        var classId = Guid.NewGuid();
        var objectId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.Empty.ToString(), // Empty GUID
            Methods = [],
            Attributes = []
        };

        var objectClass = new SimClass { Id = objectId, Name = "Object" };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Default);
        _mockExecutionService = new Mock<IExecutionService>(MockBehavior.Default);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockExecutionService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(objectId))
            .Returns(objectClass);

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()));

        var result = _simClassAdapter.UpdateSimClass(request, classId);

        Assert.IsNotNull(result);
        _mockSimClassService.Verify(s => s.GetSimClassById(objectId), Times.Once);
    }
}

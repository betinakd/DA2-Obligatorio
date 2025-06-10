using Adapter;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using IAdapter.Exceptions;
using IBusinessLogic;
using Models.Enums;
using Models.Request;
using Models.Response;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class SimClassAdapterTest
{
    private Mock<ISimClassService>? _mockSimClassService;
    private Mock<IMethodService>? _mockMethodService;
    private SimClassAdapter? _simClassAdapter;
    private Mock<INamespaceService>? _mockNamespaceService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _mockNamespaceService = new Mock<INamespaceService>(MockBehavior.Strict);
        _mockNamespaceService = new Mock<INamespaceService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);
    }

    [TestMethod]
    public void GetAllSimClasses_ShouldReturnListOfSimClassResponses()
    {
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "NamespaceB", BaseNamespaceId = null };

        var simClasses = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "ClassA", Namespace = simNamespace, NamespaceId = simNamespace.Id },
            new SimClass { Id = Guid.NewGuid(), Name = "ClassB", Namespace = simNamespace, NamespaceId = simNamespace.Id },
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
        var simNamespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(expectedNamespace))
                .Returns(expectedNamespace);

        var simClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "ValidClass",
            State = SimAccesibility.Normal,
            NamespaceId = expectedNamespace.Id,
            Namespace = expectedNamespace
        };

        var request = new SimClassRequestCreate
        {
            Name = "ValidClass",
            IdBaseClass = Guid.NewGuid().ToString(),
            State = SimModelsAccesibility.Normal,
            IdBaseNamespace = expectedNamespace.Id.ToString(),
        };
        _mockSimClassService
            ?.Setup(service => service.CreateSimClass(
                request.Name,
                SimAccesibility.Normal,
                request.BaseClassId,
                expectedNamespace.Id))
            .Returns(simClass);

        var result = _simClassAdapter?.CreateSimClass(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Class created successfully", result!.Message);
        Assert.AreEqual(simClass.Id, result.SimClass?.Id);
        Assert.AreEqual(simClass.Name, result.SimClass?.Name);

        _mockSimClassService?.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId, expectedNamespace.Id), Times.Once);
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
        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "NamespaceB", BaseNamespaceId = null };

        var simClass = new SimClass { Id = simClassId, Name = "ExistingClass", Namespace = simNamespace, NamespaceId = simNamespace.Id };
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
        var simNamespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(expectedNamespace))
            .Returns(expectedNamespace);

        var request = new SimClassRequestCreate
        {
            Name = "TestClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString(),
            IdBaseNamespace = expectedNamespace.Id.ToString(),
        };
        _mockSimClassService!
            .Setup(service => service.CreateSimClass(
                request.Name,
                SimAccesibility.Normal,
                request.BaseClassId,
                expectedNamespace.Id))
            .Throws(new InUseValueLogic("Class is already in use."));

        var exception = Assert.ThrowsException<InUseValueAdapter>(() =>
            _simClassAdapter!.CreateSimClass(request));

        Assert.AreEqual("Class is already in use.", exception.Message);

        _mockSimClassService.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId, expectedNamespace.Id), Times.Once);
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
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(@expectedNamespace))
            .Returns(expectedNamespace);

        var request = new SimClassRequestCreate
        {
            Name = "Invalid-Name-With-Chars",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString(),
            IdBaseNamespace = expectedNamespace.Id.ToString(),
        };
        _mockSimClassService!
            .Setup(service => service.CreateSimClass(
                request.Name,
                SimAccesibility.Normal,
                request.BaseClassId,
                expectedNamespace.Id))
            .Throws(new InvalidAttributeLogic("Name contains invalid characters."));

        var exception = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simClassAdapter!.CreateSimClass(request));

        Assert.AreEqual("Name contains invalid characters.", exception.Message);

        _mockSimClassService.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId, expectedNamespace.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueAdapter))]
    public void CreateSimClass_ShouldThrowNonExistentValueAdapter_WhenServiceThrowsNonExistentValueLogic()
    {
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(expectedNamespace))
            .Returns(expectedNamespace);

        var request = new SimClassRequestCreate
        {
            Name = "TestClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString(),
            IdBaseNamespace = expectedNamespace.Id.ToString(),
        };
        _mockSimClassService!
            .Setup(service => service.CreateSimClass(
                request.Name,
                SimAccesibility.Normal,
                request.BaseClassId,
                expectedNamespace.Id))
            .Throws(new NonExistentValueLogic("Base class not found."));
        _simClassAdapter!.CreateSimClass(request);

        _mockSimClassService.Verify(service => service.CreateSimClass(request.Name, SimAccesibility.Normal, request.BaseClassId, expectedNamespace.Id), Times.Once);
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
            Attributes = [],
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
            Attributes = [],
        };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

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
            Attributes = [],
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Sealed };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

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
            Attributes = [],
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Normal };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

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
    public void UpdateSimClass_ShouldSetDefaultObjectId_WhenBaseClassIdIsEmpty()
    {
        var classId = Guid.NewGuid();
        var objectId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "NamespaceObject" };
        var objectClass = new SimClass
        {
            Id = objectId,
            Name = "Object",
            Namespace = simNamespace,
            NamespaceId = simNamespace.Id,
            Methods = [],
            Attributes = [],
            Implements = [],
        };

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.Empty.ToString(),
            Methods = [],
            Attributes = [],
        };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(objectId))
            .Returns(objectClass);

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Callback<SimClass>(c =>
            {
                c.Methods ??= [];
                c.Attributes ??= [];
                c.Implements ??= [];
                c.Namespace ??= simNamespace;
            })
            .Returns((SimClass c) => c);

        var result = _simClassAdapter.UpdateSimClass(request, classId);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.SimClass);
        Assert.AreEqual("UpdatedClass", result.SimClass.Name);
        _mockSimClassService.Verify(s => s.GetSimClassById(objectId), Times.Once);
        _mockSimClassService.Verify(s => s.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldProcessImplementsCorrectly()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();
        var interfaceId1 = Guid.NewGuid();
        var interfaceId2 = Guid.NewGuid();

        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "NamespaceA" };

        var request = new SimClassRequestUpdate
        {
            Name = "UpdatedClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = [],
            Implements =
            [
                new InterfaceRequestUpdate { IdInterface = interfaceId1.ToString() },
            new InterfaceRequestUpdate { IdInterface = interfaceId2.ToString() }
            ],
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Normal, Namespace = simNamespace, NamespaceId = simNamespace.Id };
        var interfaceClass1 = new SimClass { Id = interfaceId1, Name = "Interface1", State = SimAccesibility.Interface, Namespace = simNamespace, NamespaceId = simNamespace.Id, Methods = [] };
        var interfaceClass2 = new SimClass { Id = interfaceId2, Name = "Interface2", State = SimAccesibility.Interface, Namespace = simNamespace, NamespaceId = simNamespace.Id, Methods = [] };

        var updatedClass = new SimClass
        {
            Id = classId,
            Name = "UpdatedClass",
            BaseClassId = baseClassId,
            Namespace = simNamespace,
            NamespaceId = simNamespace.Id,
            Methods = [],
            Attributes = [],
            Implements = [interfaceClass1, interfaceClass2],
        };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(interfaceId1))
            .Returns(interfaceClass1);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(interfaceId2))
            .Returns(interfaceClass2);

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Callback<SimClass>(c =>
            {
                c.Methods ??= [];
                c.Attributes ??= [];
                c.Implements ??= [interfaceClass1, interfaceClass2];
                c.Namespace ??= simNamespace;
            })
            .Returns(updatedClass);

        var result = _simClassAdapter.UpdateSimClass(request, classId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Class updated successfully", result.Message);
        Assert.IsNotNull(result.SimClass);
        Assert.AreEqual(classId, result.SimClass.Id);
        Assert.AreEqual("UpdatedClass", result.SimClass.Name);

        _mockSimClassService.Verify(s => s.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(interfaceId1), Times.Once);
        _mockSimClassService.Verify(s => s.GetSimClassById(interfaceId2), Times.Once);
        _mockSimClassService.Verify(s => s.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInvalidAttributeAdapter_WhenInvalidAttributeLogicOccurs()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();

        var request = new SimClassRequestUpdate
        {
            Name = "InvalidClassName",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            Methods = [],
            Attributes = [],
        };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Throws(new InvalidAttributeLogic("Invalid attribute detected"));

        var exception = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simClassAdapter.UpdateSimClass(request, classId));

        Assert.AreEqual("Invalid attribute detected", exception.Message);
        _mockSimClassService.Verify(s => s.GetSimClassById(baseClassId), Times.Once);
    }

    [TestMethod]
    public void CreateSimClass_InvalidAttributeAdapter_WhenNamespaceIdIsNull()
    {
        var request = new SimClassRequestCreate
        {
            Name = "TestClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = Guid.NewGuid().ToString(),
            IdBaseNamespace = Guid.Empty.ToString(),
        };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.GetNamespaceById(Guid.Empty))
            .Throws(new InvalidAttributeLogic("Namespace ID cannot be null."));

        _mockSimClassService
        .Setup(s => s.CreateSimClass(
            "TestClass",
            SimAccesibility.Normal,
            It.IsAny<Guid>(),
            Guid.Empty))
        .Throws(new InvalidAttributeLogic("Namespace ID cannot be null."));

        var exception = Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _simClassAdapter.CreateSimClass(request));

        Assert.AreEqual("Namespace ID cannot be null.", exception.Message);
    }

    [TestMethod]
    public void CreateSimClass_NonExistentValueAdapter_WhenNamespaceDoesNotExist()
    {
        var baseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var request = new SimClassRequestCreate
        {
            Name = "TestClass",
            State = SimModelsAccesibility.Normal,
            IdBaseClass = baseClassId.ToString(),
            IdBaseNamespace = Guid.NewGuid().ToString(),
        };

        _mockNamespaceService
            ?.Setup(service => service.GetNamespaceById(request.BaseNamespaceId))
            .Returns((SimNamespace)null);
        _mockSimClassService
            .Setup(s => s.CreateSimClass(
                "TestClass",
                SimAccesibility.Normal,
                baseClassId,
                request.BaseNamespaceId))
            .Throws(new NonExistentValueLogic($"Namespace with ID {request.BaseNamespaceId} does not exist."));

        var exception = Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _simClassAdapter.CreateSimClass(request));
    }

    [TestMethod]
    public void UpdateSimClass_ShouldReturnSuccessResponse_WhenUpdateIsSuccessful()
    {
        var classId = Guid.NewGuid();
        var baseClassId = Guid.NewGuid();
        var objTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var instanceId = Guid.NewGuid();
        var typeId = Guid.NewGuid();

        var simNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "NamespaceA" };

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
                    new ParameterRequest { Name = "param1", IdReference = objTypeId.ToString() }
                ],
            }

            ],
            Attributes =
            [
                new AttributeRequest
            {
                Name = "TestAttribute",
                Privacity = SimModelsPrivacity.Private,
                IdReference = typeId.ToString(),
                IdInstance = instanceId.ToString()
            }

            ],
        };

        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass", State = SimAccesibility.Normal, Namespace = simNamespace, NamespaceId = simNamespace.Id };
        var objectClass = new SimClass { Id = objTypeId, Name = "Object", Namespace = simNamespace, NamespaceId = simNamespace.Id };
        var typeClass = new SimClass { Id = typeId, Name = "TypeClass", Namespace = simNamespace, NamespaceId = simNamespace.Id };
        var instanceClass = new SimClass { Id = instanceId, Name = "InstanceClass", Namespace = simNamespace, NamespaceId = simNamespace.Id };

        var updatedClass = new SimClass
        {
            Id = classId,
            Name = "UpdatedClass",
            BaseClassId = baseClassId,
            Namespace = simNamespace,
            NamespaceId = simNamespace.Id,
            Methods = [],
            Attributes = [],
            Implements = [],
        };

        _mockSimClassService = new Mock<ISimClassService>(MockBehavior.Strict);
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _simClassAdapter = new SimClassAdapter(_mockSimClassService.Object, _mockMethodService.Object);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(baseClassId))
            .Returns(baseClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(objTypeId))
            .Returns(objectClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(typeId))
            .Returns(typeClass);

        _mockSimClassService
            .Setup(s => s.GetSimClassById(instanceId))
            .Returns(instanceClass);

        _mockSimClassService
            .Setup(s => s.ValidPolymorphism(typeClass, instanceClass));

        _mockMethodService
            .Setup(s => s.IsValidVirtualOverride(It.IsAny<SimClass>(), It.IsAny<SimMethod>()));

        _mockSimClassService
            .Setup(s => s.UpdateSimClass(It.IsAny<SimClass>()))
            .Callback<SimClass>(c =>
            {
                c.Methods ??= [];
                c.Attributes ??= [];
                c.Implements ??= [];
                c.Namespace ??= simNamespace;
                updatedClass = c;
            })
            .Returns(() => updatedClass);

        var result = _simClassAdapter.UpdateSimClass(request, classId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Class updated successfully", result.Message);
        Assert.IsNotNull(result.SimClass);
        Assert.AreEqual(classId, result.SimClass.Id);
        Assert.AreEqual("UpdatedClass", result.SimClass.Name);

        _mockSimClassService.Verify(s => s.UpdateSimClass(It.IsAny<SimClass>()), Times.Once);

        _mockMethodService.Verify(s => s.IsValidVirtualOverride(It.IsAny<SimClass>(), It.IsAny<SimMethod>()), Times.Once);

        _mockSimClassService.Verify(s => s.ValidPolymorphism(typeClass, instanceClass), Times.Once);
    }
}

using BusinessLogic;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using IBusinessLogic;
using IDataAccess;
using Models.Request;
using Moq;

namespace Tests.BusinessLogic;

[TestClass]
public class SimClassServiceTest
{
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private Mock<ISimAttributeDataAccess>? _mockSimAttributeDataAccess;
    private Mock<IExecutionDataAccess>? _mockExecutionDataAccess;
    private Mock<INamespaceService>? _mockNamespaceService;
    private SimClassService? _simClassService;

    [TestInitialize]
    public void Initialize()
    {
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>(MockBehavior.Strict);
        _mockSimAttributeDataAccess = new Mock<ISimAttributeDataAccess>(MockBehavior.Strict);
        _mockExecutionDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _mockNamespaceService = new Mock<INamespaceService>(MockBehavior.Strict);

        _simClassService = new SimClassService(
            _mockSimClassDataAccess.Object,
            _mockSimAttributeDataAccess.Object,
            _mockExecutionDataAccess.Object,
            _mockNamespaceService.Object);
    }

    [TestMethod]
    public void CreateSimClass_ShouldReturnSimClassWithCorrectProperties()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockSimClassDataAccess.Setup(da => da.CreateSimClass(It.IsAny<SimClass>()));

        var @namespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService?.Setup(service => service.CreateNamespace(@namespace)).Returns(expectedNamespace);
        _mockNamespaceService?.Setup(s => s.GetNamespaceById(expectedNamespace.Id)).Returns(expectedNamespace);
        _mockNamespaceService?.Setup(s => s.NameAlreadyInNamespace_Validation(expectedNamespace.Id, "TestClass")).Returns(false);

        var result = _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId, expectedNamespace.Id);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.CreateSimClass(It.Is<SimClass>(
            s => s.Name == "TestClass" &&
                 s.BaseClass == baseClass &&
                 s.Id != Guid.Empty &&
                 s.State == SimAccesibility.Normal)), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
        Assert.AreEqual(baseClass, result.BaseClass);
        Assert.AreNotEqual(Guid.Empty, result.Id);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowDuplicateValueLogic_WhenNameAlreadyExists()
    {
        var baseClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);

        var @namespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(@namespace))
                .Returns(expectedNamespace);
        _mockNamespaceService.Setup(s => s.NameAlreadyInNamespace_Validation(expectedNamespace.Id, "TestClass")).Returns(false);

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId, expectedNamespace.Id));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowNonExistentValueLogic_WhenBaseClassDoesNotExist()
    {
        var baseClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(false);

        var @namespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(@namespace))
                .Returns(expectedNamespace);
        _mockNamespaceService.Setup(s => s.NameAlreadyInNamespace_Validation(expectedNamespace.Id, "TestClass")).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId, expectedNamespace.Id));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void CreateSimClass_ShouldThrowInvalidAttributeLogic_WhenSimClassInvalidAttributeIsThrown()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockSimClassDataAccess.Setup(da => da.CreateSimClass(It.IsAny<SimClass>())).Throws(new InvalidAttributeDomain("Invalid attribute"));

        var @namespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(@namespace))
                .Returns(expectedNamespace);
        _mockNamespaceService.Setup(s => s.GetNamespaceById(expectedNamespace.Id)).Returns(expectedNamespace);
        _mockNamespaceService.Setup(s => s.NameAlreadyInNamespace_Validation(expectedNamespace.Id, "TestClass")).Returns(false);

        Assert.ThrowsException<InvalidAttributeLogic>(() =>
            _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId, expectedNamespace.Id));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.CreateSimClass(It.IsAny<SimClass>()), Times.Once);
    }

    [TestMethod]
    public void CreateSimClass_ShouldCreateAndReturnSimClass_WhenAllIsValid()
    {
        var baseClassId = Guid.NewGuid();
        var baseClass = new SimClass { Id = baseClassId, Name = "BaseClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassName("TestClass")).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(baseClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(baseClassId)).Returns(baseClass);
        _mockSimClassDataAccess.Setup(da => da.CreateSimClass(It.IsAny<SimClass>()));

        var @namespace = new NamespaceRequest { Name = "Namespace", BaseNamespaceId = null };
        var expectedNamespace = new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace", BaseNamespaceId = null };
        _mockNamespaceService
            ?.Setup(service => service.CreateNamespace(@namespace))
                .Returns(expectedNamespace);
        _mockNamespaceService.Setup(s => s.GetNamespaceById(expectedNamespace.Id)).Returns(expectedNamespace);
        _mockNamespaceService.Setup(s => s.NameAlreadyInNamespace_Validation(expectedNamespace.Id, "TestClass")).Returns(false);

        var result = _simClassService.CreateSimClass("TestClass", SimAccesibility.Normal, baseClassId, expectedNamespace.Id);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassName("TestClass"), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(baseClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.CreateSimClass(It.Is<SimClass>(
            s => s.Name == "TestClass" &&
                 s.BaseClass == baseClass &&
                 s.Id != Guid.Empty &&
                 s.State == SimAccesibility.Normal)), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestClass", result.Name);
        Assert.AreEqual(baseClass, result.BaseClass);
        Assert.AreNotEqual(Guid.Empty, result.Id);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldDelete_WhenSimClassExistsAndNotInUse()
    {
        var simClassId = Guid.NewGuid();

        var simClass = new SimClass
        {
            Id = simClassId,
            Name = "TestClass",
            Attributes = [],
            Methods = [],
        };

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.DeleteSimClass(simClassId));

        _simClassService.DeleteSimClass(simClassId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.DeleteSimClass(simClassId), Times.Once);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldThrowNonExistentValueLogic_WhenSimClassDoesNotExist()
    {
        var simClassId = Guid.NewGuid();
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.DeleteSimClass(simClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(It.IsAny<Guid>()), Times.Never);
        _mockSimClassDataAccess.Verify(da => da.DeleteSimClass(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void GetAllSimClasses_ShouldReturnAllSimClasses()
    {
        var simClasses = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "Class1" },
            new SimClass { Id = Guid.NewGuid(), Name = "Class2" },
        };
        _mockSimClassDataAccess.Setup(da => da.GetAllSimClasses()).Returns(simClasses);

        var result = _simClassService.GetAllSimClasses();

        _mockSimClassDataAccess.Verify(da => da.GetAllSimClasses(), Times.Once);
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(simClasses, result.ToList());
    }

    [TestMethod]
    public void GetSimClassById_ShouldReturnSimClass()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "TestClass" };
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);

        var result = _simClassService.GetSimClassById(simClassId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
    }

    [TestMethod]
    public void GetSimClassById_ShouldThrowNonExistentValueLogic_WhenNotExists()
    {
        var simClassId = Guid.NewGuid();

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.GetSimClassById(simClassId));
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldUpdateAndReturnSimClass_WhenExists()
    {
        var namespaceId = Guid.NewGuid();
        var simClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedClass",
            Attributes = [],
            Methods = [],
            NamespaceId = namespaceId,
        };
        var simNamespace = new SimNamespace { Id = namespaceId, Name = "Namespace" };
        _mockNamespaceService.Setup(da => da.GetNamespaceById(namespaceId)).Returns(simNamespace);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClass.Id)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClass.Id)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClass.Id)).Returns(false);
        _mockSimClassDataAccess.Setup(da => da.UpdateSimClass(simClass));

        var result = _simClassService.UpdateSimClass(simClass);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(simClass), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual(simClass, result);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowNonExistentValueLogic_WhenNotExists()
    {
        var namespaceId = Guid.NewGuid();
        var simClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedClass",
            Attributes = [],
            Methods = [],
            NamespaceId = namespaceId,
        };
        var simNamespace = new SimNamespace { Id = namespaceId, Name = "Namespace" };
        _mockNamespaceService.Setup(da => da.GetNamespaceById(namespaceId)).Returns(simNamespace);

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClass.Id)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.UpdateSimClass(simClass));
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void UpdateSimClass_ShouldThrowInUseValueLogic_WhenSimClassIsInUse()
    {
        var namespaceId = Guid.NewGuid();
        var simClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedClass",
            Attributes = [],
            Methods = [],
            NamespaceId = namespaceId,
        };
        var simNamespace = new SimNamespace { Id = namespaceId, Name = "Namespace" };
        _mockNamespaceService.Setup(da => da.GetNamespaceById(namespaceId)).Returns(simNamespace);

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClass.Id)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClass.Id)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClass.Id)).Returns(true);

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.UpdateSimClass(simClass));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClass.Id), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void DeleteSimClass_ShouldThrowInUseValueLogic_WhenSimClassIsInUse()
    {
        var simClassId = Guid.NewGuid();

        var simClass = new SimClass
        {
            Id = simClassId,
            Name = "TestClass",
            Attributes = [],
            Methods = [],
        };

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(simClassId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(true);

        Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.DeleteSimClass(simClassId));

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.DeleteSimClass(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void InUseByOther_ShouldReturnTrue_WhenNothingIsInUse()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass
        {
            Id = simClassId,
            Name = "TestClass",
            Attributes = [],
            Methods = [],
        };

        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(false);

        var result = _simClassService.InUseByOther(simClassId);

        Assert.IsTrue(result);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClassId), Times.Once);
    }

    [TestMethod]
    public void InUseByOther_ShouldThrowInUseValueLogic_WhenSimClassIsInUse()
    {
        var simClassId = Guid.NewGuid();
        var simClass = new SimClass { Id = simClassId, Name = "TestClass" };

        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(true);

        var exception = Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.InUseByOther(simClassId));

        Assert.AreEqual("SimClass is in use as type or baseClass in others entities and cannot be updated.", exception.Message);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.InUseByOther(simClassId), Times.Once);
    }

    [TestMethod]
    public void InUseByOther_ShouldThrowInUseValueLogic_WhenAttributeIsInUse()
    {
        var simClassId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        var attribute = new SimAttribute { Id = attributeId, Name = "TestAttribute" };
        var simClass = new SimClass
        {
            Id = simClassId,
            Name = "TestClass",
            Attributes = [attribute],
            Methods = [],
        };

        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(false);
        _mockSimAttributeDataAccess.Setup(da => da.InUseByOther(attributeId)).Returns(true);

        var exception = Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.InUseByOther(simClassId));

        Assert.AreEqual("An attribute is in used as reference by an invocation and cannot be updated.", exception.Message);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        _mockSimAttributeDataAccess.Verify(da => da.InUseByOther(attributeId), Times.Once);
    }

    [TestMethod]
    public void InUseByOther_ShouldThrowInUseValueLogic_WhenMethodIsInUse()
    {
        var simClassId = Guid.NewGuid();
        var methodId = Guid.NewGuid();

        var method = new SimMethod { Id = methodId, Name = "TestMethod" };
        var simClass = new SimClass
        {
            Id = simClassId,
            Name = "TestClass",
            Attributes = [],
            Methods = [method],
        };

        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(simClassId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.InUseByOther(simClassId)).Returns(false);
        _mockExecutionDataAccess.Setup(da => da.MethodIsInUseByInheritingInvocations(methodId)).Returns(true);

        var exception = Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.InUseByOther(simClassId));

        Assert.AreEqual("Method is in use by invocations and cannot be updated.", exception.Message);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(simClassId), Times.Once);
        _mockExecutionDataAccess.Verify(da => da.MethodIsInUseByInheritingInvocations(methodId), Times.Once);
    }

    [TestMethod]
    public void AddInterface_ShouldAddInterfaceToSimClass_WhenBothExist()
    {
        var classId = Guid.NewGuid();
        var interfaceId = Guid.NewGuid();
        var namespaceId = Guid.NewGuid();

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            Implements = [],
            NamespaceId = namespaceId,
        };
        var interfaceToAdd = new SimClass
        {
            Id = interfaceId,
            Name = "ITestInterface",
            State = SimAccesibility.Interface,
        };
        var simNamespace = new SimNamespace { Id = namespaceId, Name = "Namespace" };
        _mockNamespaceService.Setup(da => da.GetNamespaceById(namespaceId)).Returns(simNamespace);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(interfaceId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(classId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(interfaceId)).Returns(interfaceToAdd);
        _mockSimClassDataAccess.Setup(da => da.UpdateSimClass(It.IsAny<SimClass>()));

        _mockSimClassDataAccess.Setup(da => da.InUseByOther(classId)).Returns(false);

        _mockSimAttributeDataAccess.Setup(da =>
            da.InUseByOther(It.IsAny<Guid>())).Returns(false);
        _mockExecutionDataAccess.Setup(da =>
            da.MethodIsInUseByInheritingInvocations(It.IsAny<Guid>())).Returns(false);

        var result = _simClassService.AddInterface(classId, interfaceId);

        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(classId), Times.AtLeastOnce);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(interfaceId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(classId), Times.AtLeastOnce);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(interfaceId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.Is<SimClass>(s =>
            s.Implements.Contains(interfaceToAdd))), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Implements.Count);
        Assert.IsTrue(result.Implements.Contains(interfaceToAdd));
    }

    [TestMethod]
    public void AddInterface_ShouldThrowNonExistentValueLogic_WhenSimClassDoesNotExist()
    {
        var classId = Guid.NewGuid();
        var interfaceId = Guid.NewGuid();

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(false);

        var exception = Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.AddInterface(classId, interfaceId));

        Assert.AreEqual("SimClass not found.", exception.Message);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(classId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(It.IsAny<Guid>()), Times.Never);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void AddInterface_ShouldThrowNonExistentValueLogic_WhenInterfaceDoesNotExist()
    {
        var classId = Guid.NewGuid();
        var interfaceId = Guid.NewGuid();

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(interfaceId)).Returns(false);

        var exception = Assert.ThrowsException<NonExistentValueLogic>(() =>
            _simClassService.AddInterface(classId, interfaceId));

        Assert.AreEqual("Interface not found.", exception.Message);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(classId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(interfaceId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(It.IsAny<Guid>()), Times.Never);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void AddInterface_ShouldThrowInUseValueLogic_WhenInterfaceAlreadyAdded()
    {
        var classId = Guid.NewGuid();
        var interfaceId = Guid.NewGuid();

        var interfaceToAdd = new SimClass
        {
            Id = interfaceId,
            Name = "ITestInterface",
            State = SimAccesibility.Interface,
        };

        var simClass = new SimClass
        {
            Id = classId,
            Name = "TestClass",
            Implements = [interfaceToAdd],
        };

        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(classId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.ExistSimClassById(interfaceId)).Returns(true);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(classId)).Returns(simClass);
        _mockSimClassDataAccess.Setup(da => da.GetSimClassById(interfaceId)).Returns(interfaceToAdd);

        var exception = Assert.ThrowsException<InUseValueLogic>(() =>
            _simClassService.AddInterface(classId, interfaceId));

        Assert.AreEqual("Interface already added.", exception.Message);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(classId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.ExistSimClassById(interfaceId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(classId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.GetSimClassById(interfaceId), Times.Once);
        _mockSimClassDataAccess.Verify(da => da.UpdateSimClass(It.IsAny<SimClass>()), Times.Never);
    }

    [TestMethod]
    public void ClassInheritAttribute_AttributeInherited_DoesNotThrowException()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()))
            .Returns(true);

        _simClassService!.ClassInheritAttribute(classId, attributeId);

        _mockSimClassDataAccess.Verify(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void ClassInheritAttribute_AttributeNotInherited_ThrowsNonExistentValueLogic()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()))
            .Returns(false);

        _simClassService!.ClassInheritAttribute(classId, attributeId);
    }

    [TestMethod]
    public void ClassInheritAttribute_VerifiesExceptionMessage()
    {
        var classId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();

        _mockSimClassDataAccess!
            .Setup(m => m.ClassInheritAttribute(classId, attributeId, It.IsAny<int>()))
            .Returns(false);

        var exception = Assert.ThrowsException<NonExistentValueLogic>(
            () => _simClassService!.ClassInheritAttribute(classId, attributeId));

        Assert.AreEqual("Attribute not reacheable from method.", exception.Message);
    }

    [TestMethod]
    public void ValidPolymorphism_ShouldThrow_WhenDerivedIsInterface()
    {
        var baseClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "BaseClass",
            State = SimAccesibility.Normal,
        };

        var derivedClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "DerivedInterface",
            State = SimAccesibility.Interface,
        };

        var ex = Assert.ThrowsException<InvalidAttributeLogic>(() =>
            _simClassService.ValidPolymorphism(baseClass, derivedClass));

        Assert.AreEqual("Polymorphic inheritance is not allowed when the derived type is an interface.", ex.Message);
    }

    [TestMethod]
    public void ValidPolymorphism_ShouldThrow_WhenDerivedIsAbstract()
    {
        var baseClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "BaseClass",
            State = SimAccesibility.Normal,
        };

        var derivedClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "DerivedAbstract",
            State = SimAccesibility.Abstract,
        };

        var ex = Assert.ThrowsException<InvalidAttributeLogic>(() =>
            _simClassService.ValidPolymorphism(baseClass, derivedClass));

        Assert.AreEqual("Polymorphic inheritance is not allowed when the base type is abstract.", ex.Message);
    }

    [TestMethod]
    public void ValidPolymorphism_ShouldThrow_WhenReferenceClassIsBaseOfInstanceClass()
    {
        var baseClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "BaseClass",
            State = SimAccesibility.Normal,
        };

        var derivedClass = new SimClass
        {
            Id = Guid.NewGuid(),
            Name = "DerivedClass",
            State = SimAccesibility.Normal,
        };

        _mockSimClassDataAccess!
            .Setup(m => m.IsClassBaseOfOrSameAs(baseClass, derivedClass))
            .Returns(false);

        var ex = Assert.ThrowsException<InvalidAttributeLogic>(() =>
            _simClassService.ValidPolymorphism(baseClass, derivedClass));

        Assert.AreEqual("Reference class is not base of instance class.", ex.Message);
    }

    [TestMethod]
    public void GetClassesOfNamespaces_ShouldReturnClasses_WhenNamespaceExists()
    {
        var namespaceId = Guid.NewGuid();
        var classes = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "Class1", NamespaceId = namespaceId },
            new SimClass { Id = Guid.NewGuid(), Name = "Class2", NamespaceId = namespaceId },
            new SimClass { Id = Guid.NewGuid(), Name = "Class3", NamespaceId = Guid.NewGuid() },
        };

        _mockNamespaceService?.Setup(ns => ns.GetNamespaceById(namespaceId)).Returns(new SimNamespace { Id = namespaceId });
        _mockSimClassDataAccess?.Setup(da => da.GetAllSimClasses()).Returns(classes);

        var result = _simClassService?.GetClassesOfNamespaces(namespaceId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(c => c.NamespaceId == namespaceId));
    }

    [TestMethod]
    public void GetClassesOfNamespaces_ShouldThrowNonExistentValueLogic_WhenNamespaceDoesNotExist()
    {
        var namespaceId = Guid.NewGuid();
        _mockNamespaceService?.Setup(ns => ns.GetNamespaceById(namespaceId)).Returns((SimNamespace)null);

        Assert.ThrowsException<NonExistentValueLogic>(() => _simClassService?.GetClassesOfNamespaces(namespaceId));

        _mockNamespaceService?.Verify(ns => ns.GetNamespaceById(namespaceId), Times.Once);
        _mockSimClassDataAccess?.Verify(da => da.GetAllSimClasses(), Times.Never);
    }

    [TestMethod]
    public void GetClassesOfNamespaces_ShouldThrowInvalidAttributeLogic_WhenIdIsNull()
    {
        var namespaceId = Guid.Empty;

        _mockNamespaceService?.Setup(ns => ns.GetNamespaceById(namespaceId))
            .Throws(new InvalidAttributeLogic("Namespace can't be empty."));

        Assert.ThrowsException<InvalidAttributeLogic>(() => _simClassService?.GetClassesOfNamespaces(namespaceId));

        _mockNamespaceService?.Verify(ns => ns.GetNamespaceById(namespaceId), Times.Once);
        _mockSimClassDataAccess?.Verify(da => da.GetAllSimClasses(), Times.Never);
    }

    [TestMethod]
    public void GetClassesOfNamespaces_ShouldReturnEmptyList_WhenNamespaceExists()
    {
        var namespaceId = Guid.NewGuid();
        var classes = new List<SimClass>();

        _mockNamespaceService?.Setup(ns => ns.GetNamespaceById(namespaceId)).Returns(new SimNamespace { Id = namespaceId });
        _mockSimClassDataAccess?.Setup(da => da.GetAllSimClasses()).Returns(classes);

        var result = _simClassService?.GetClassesOfNamespaces(namespaceId);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}

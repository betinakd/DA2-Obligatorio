using BusinessLogic;
using BusinessLogic.Exceptions;
using Domain;
using IBusinessLogic;
using IDataAccess;
using Models.Request;
using Moq;

[TestClass]
public class NamespaceServiceTest
{
    private Mock<INamespaceDataAccess>? _mockNamespaceDataAccess;
    private Mock<ISimClassDataAccess>? _mockSimClassDataAccess;
    private NamespaceService? _namespaceService;

    [TestInitialize]
    public void Setup()
    {
        _mockNamespaceDataAccess = new Mock<INamespaceDataAccess>();
        _mockSimClassDataAccess = new Mock<ISimClassDataAccess>();
        _namespaceService = new NamespaceService(_mockNamespaceDataAccess.Object, _mockSimClassDataAccess.Object);
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowInvalidAttributeLogic_WhenNameIsEmpty()
    {
        var request = new NamespaceRequest { Name = string.Empty, BaseNamespaceId = null };

        Assert.ThrowsException<InvalidAttributeLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowNonExistentValueLogic_WhenBaseNamespaceDoesNotExist()
    {
        var request = new NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = Guid.NewGuid() };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(request.BaseNamespaceId.Value)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowInvalidAttributeLogic_WhenNamespaceAlreadyExists()
    {
        var request = new NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = null };

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns(new List<SimNamespace>
        {
            new SimNamespace { Name = "TestNamespace", BaseNamespaceId = null }
        });

        Assert.ThrowsException<InvalidAttributeLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldCreateNamespace_WhenValidRequest()
    {
        var request = new NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = null };

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns(new List<SimNamespace>());
        _mockNamespaceDataAccess.Setup(x => x.CreateNamespace(It.IsAny<SimNamespace>()));

        var result = _namespaceService!.CreateNamespace(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestNamespace", result.Name);
        Assert.IsNull(result.BaseNamespaceId);
    }

    [TestMethod]
    public void AddClassInNamespace_ShouldAddClass_WhenValidRequest()
    {
        var namespaceId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var request = new NamespaceElementAdd_Request { ClassId = classId };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(namespaceId)).Returns(true);
        _mockSimClassDataAccess!.Setup(x => x.ExistSimClassById(classId)).Returns(true);
        _mockNamespaceDataAccess.Setup(x => x.AddClassInNamespace(namespaceId, classId));

        var result = _namespaceService!.AddClassInNamespace(namespaceId, request);

        Assert.AreEqual($"Class with ID {classId} added to namespace with ID {namespaceId}.", result);
    }

    [TestMethod]
    public void AddClassInNamespace_ShouldThrowNonExistentValueLogic_WhenNamespaceDoesNotExist()
    {
        var namespaceId = Guid.NewGuid();
        var request = new NamespaceElementAdd_Request { ClassId = Guid.NewGuid() };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(namespaceId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() => _namespaceService!.AddClassInNamespace(namespaceId, request));
    }

    [TestMethod]
    public void AddClassInNamespace_ShouldThrowNonExistentValueLogic_WhenClassDoesNotExist()
    {
        var namespaceId = Guid.NewGuid();
        var request = new NamespaceElementAdd_Request { ClassId = Guid.NewGuid() };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(namespaceId)).Returns(true);
        _mockSimClassDataAccess!.Setup(x => x.ExistSimClassById(request.ClassId)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() => _namespaceService!.AddClassInNamespace(namespaceId, request));
    }
}

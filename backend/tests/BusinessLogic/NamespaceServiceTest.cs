using BusinessLogic;
using Domain;
using Domain.Exceptions;
using IBusinessLogic.Exceptions;
using IDataAccess;
using Moq;

namespace Tests.BusinessLogic;

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
        _namespaceService = new NamespaceService(_mockNamespaceDataAccess.Object);
    }

    [TestMethod]
    public void GetNamespaceById_ShouldReturnNamespace_WhenExists()
    {
        var namespaceId = Guid.NewGuid();
        var expectedNamespace = new SimNamespace { Id = namespaceId, Name = "TestNamespace" };

        _mockNamespaceDataAccess!.Setup(x => x.GetNamespaceById(namespaceId)).Returns(expectedNamespace);

        var result = _namespaceService!.GetNamespaceById(namespaceId);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedNamespace.Id, result.Id);
        Assert.AreEqual(expectedNamespace.Name, result.Name);
    }

    [TestMethod]
    public void GetAllNamespaces_ShouldReturnAllNamespaces()
    {
        var namespace1Id = Guid.NewGuid();
        var expectedNamespaces = new List<SimNamespace>
        {
            new SimNamespace { Id = namespace1Id, Name = "Namespace1", BaseNamespaceId = null },
            new SimNamespace { Id = Guid.NewGuid(), Name = "Namespace2", BaseNamespaceId = namespace1Id },
        };

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns(expectedNamespaces);

        var result = _namespaceService!.GetAllNamespaces();

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedNamespaces.Count, result.Count);
        for(var i = 0; i < result.Count; i++)
        {
            Assert.AreEqual(expectedNamespaces[i].Id, result[i].Id);
            Assert.AreEqual(expectedNamespaces[i].Name, result[i].Name);
            Assert.AreEqual(expectedNamespaces[i].BaseNamespaceId, result[i].BaseNamespaceId);
        }
    }

    [TestMethod]
    public void GetAllNamespaces_ShouldReturnAllNamespaces_EmptyListCase()
    {
        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns([]);

        var result = _namespaceService!.GetAllNamespaces();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeDomain))]
    public void CreateNamespace_ShouldThrowException_WhenNameIsEmpty()
    {
        var simNamespace = new SimNamespace { Name = string.Empty, Id = Guid.NewGuid() };

        _namespaceService!.CreateNamespace(simNamespace);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void CreateNamespace_ShouldThrowException_WhenBaseNamespaceDoesNotExist()
    {
        var baseNamespaceId = Guid.NewGuid();
        var simNamespace = new SimNamespace
        {
            Name = "TestNamespace",
            Id = Guid.NewGuid(),
            BaseNamespaceId = baseNamespaceId
        };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(baseNamespaceId)).Returns(false);

        _namespaceService!.CreateNamespace(simNamespace);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttributeLogic))]
    public void CreateNamespace_ShouldThrowException_WhenNamespaceWithSameNameExists()
    {
        var simNamespace = new SimNamespace { Name = "ExistingNamespace", Id = Guid.NewGuid() };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsByName("ExistingNamespace")).Returns(true);

        _namespaceService!.CreateNamespace(simNamespace);
    }

    [TestMethod]
    public void CreateNamespace_ShouldReturnCreatedNamespace_WhenValid()
    {
        var simNamespace = new SimNamespace { Name = "NewNamespace", Id = Guid.NewGuid() };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsByName("NewNamespace")).Returns(false);
        _mockNamespaceDataAccess!.Setup(x => x.CreateNamespace(simNamespace)).Verifiable();

        var result = _namespaceService!.CreateNamespace(simNamespace);

        Assert.IsNotNull(result);
        Assert.AreEqual(simNamespace.Id, result.Id);
        Assert.AreEqual(simNamespace.Name, result.Name);
        _mockNamespaceDataAccess.Verify(x => x.CreateNamespace(simNamespace), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NonExistentValueLogic))]
    public void GetNamespaceById_ShouldThrowException_WhenNamespaceDoesNotExist()
    {
        var namespaceId = Guid.NewGuid();

        _mockNamespaceDataAccess!.Setup(x => x.GetNamespaceById(namespaceId)).Returns((SimNamespace)null);

        _namespaceService!.GetNamespaceById(namespaceId);
    }

    [TestMethod]
    public void CreateNamespace_ShouldCreateNamespace_WhenBaseNamespaceExists()
    {
        var baseNamespaceId = Guid.NewGuid();
        var simNamespace = new SimNamespace
        {
            Name = "TestNamespace",
            Id = Guid.NewGuid(),
            BaseNamespaceId = baseNamespaceId
        };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(baseNamespaceId)).Returns(true);
        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsByName("TestNamespace")).Returns(false);
        _mockNamespaceDataAccess!.Setup(x => x.CreateNamespace(simNamespace)).Verifiable();

        var result = _namespaceService!.CreateNamespace(simNamespace);

        Assert.IsNotNull(result);
        Assert.AreEqual(simNamespace.Id, result.Id);
        Assert.AreEqual(simNamespace.Name, result.Name);
        Assert.AreEqual(simNamespace.BaseNamespaceId, result.BaseNamespaceId);
        _mockNamespaceDataAccess.Verify(x => x.CreateNamespace(simNamespace), Times.Once);
    }
}

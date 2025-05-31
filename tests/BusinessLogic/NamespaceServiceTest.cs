using BusinessLogic;
using BusinessLogic.Exceptions;
using Domain;
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
        _namespaceService = new NamespaceService(_mockNamespaceDataAccess.Object);
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

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns(
        [
            new SimNamespace { Name = "TestNamespace", BaseNamespaceId = null }
        ]);

        Assert.ThrowsException<InvalidAttributeLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldCreateNamespace_WhenValidRequest()
    {
        var request = new NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = null };

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns([]);
        _mockNamespaceDataAccess.Setup(x => x.CreateNamespace(It.IsAny<SimNamespace>()));

        var result = _namespaceService!.CreateNamespace(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestNamespace", result.Name);
        Assert.IsNull(result.BaseNamespaceId);
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
    public void GetNamespaceById_ShouldThrowInvalidAttributeLogic_WhenIsEmpty()
    {
        Guid? namespaceId = null;

        _mockNamespaceDataAccess!.Setup(x => x.GetNamespaceById(namespaceId)).Throws(new NonExistentValueLogic("Namespace does not exist."));

        Assert.ThrowsException<InvalidAttributeLogic>(() => _namespaceService!.GetNamespaceById(namespaceId));
    }
}

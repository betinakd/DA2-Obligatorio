using BusinessLogic;
using BusinessLogic.Exceptions;
using Domain;
using IBusinessLogic;
using IDataAccess;
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
        var request = new Models.Request.NamespaceRequest { Name = string.Empty, BaseNamespaceId = null };

        Assert.ThrowsException<InvalidAttributeLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowNonExistentValueLogic_WhenBaseNamespaceDoesNotExist()
    {
        var request = new Models.Request.NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = Guid.NewGuid() };

        _mockNamespaceDataAccess!.Setup(x => x.NamespaceExistsById(request.BaseNamespaceId.Value)).Returns(false);

        Assert.ThrowsException<NonExistentValueLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowInvalidAttributeLogic_WhenNamespaceAlreadyExists()
    {
        var request = new Models.Request.NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = null };

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns(new List<SimNamespace>
        {
            new SimNamespace { Name = "TestNamespace", BaseNamespaceId = null }
        });

        Assert.ThrowsException<InvalidAttributeLogic>(() => _namespaceService!.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldCreateNamespace_WhenValidRequest()
    {
        var request = new Models.Request.NamespaceRequest { Name = "TestNamespace", BaseNamespaceId = null };

        _mockNamespaceDataAccess!.Setup(x => x.GetAllNamespaces()).Returns(new List<SimNamespace>());
        _mockNamespaceDataAccess.Setup(x => x.CreateNamespace(It.IsAny<SimNamespace>()));

        var result = _namespaceService!.CreateNamespace(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("TestNamespace", result.Name);
        Assert.IsNull(result.BaseNamespaceId);
    }
}

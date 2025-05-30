using Adapter;
using Adapter.Exceptions;
using BusinessLogic.Exceptions;
using Domain;
using IBusinessLogic;
using Models.Request;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class NamespaceAdapterTest
{
    private Mock<INamespaceService>? _mockNamespaceService;
    private NamespaceAdapter? _namespaceAdapter;

    [TestInitialize]
    public void Setup()
    {
        _mockNamespaceService = new Mock<INamespaceService>();
        _namespaceAdapter = new NamespaceAdapter(_mockNamespaceService.Object);
    }

    [TestMethod]
    public void CreateNamespace_ShouldReturnNamespaceResponse_WhenRequestIsValid()
    {
        var baseNamespace = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = "BaseNamespace_Test",
            BaseNamespaceId = null,
            Classes = [],
            Interfaces = []
        };
        var request = new Models.Request.NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = baseNamespace.Id
        };

        var expectedNamespace = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BaseNamespaceId = request.BaseNamespaceId,
            Classes = [],
            Interfaces = []
        };
        _mockNamespaceService?.Setup(x => x.CreateNamespace(request)).Returns(expectedNamespace);
        _mockNamespaceService?.Setup(x => x.GetNamespaceById(baseNamespace.Id)).Returns(baseNamespace);

        var response = _namespaceAdapter?.CreateNamespace(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(expectedNamespace.Id, response.Id);
        Assert.AreEqual(expectedNamespace.Name, response.Name);
        Assert.AreEqual(expectedNamespace.BaseNamespaceId, response.BaseNamespaceId);
        Assert.AreEqual(response.BaseNamespaceName, baseNamespace.Name);
        Assert.AreEqual(0, response.Classes.Count);
        Assert.AreEqual(0, response.Interfaces.Count);
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowInvalidAttributeAdapter_WhenNameIsEmpty()
    {
        var request = new NamespaceRequest
        {
            Name = string.Empty,
            BaseNamespaceId = null
        };

        _mockNamespaceService?.Setup(x => x.CreateNamespace(request)).Throws(new InvalidAttributeLogic("Namespace name cannot be empty."));

        Assert.ThrowsException<InvalidAttributeAdapter>(() => _namespaceAdapter?.CreateNamespace(request));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowInvalidAttributeAdapter_WhenNameAlreadyExist()
    {
        var request1 = new NamespaceRequest
        {
            Name = "RepeatedName",
            BaseNamespaceId = null
        };
        var expectedNamespace1 = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = request1.Name,
            Classes = [],
            Interfaces = []
        };
        _mockNamespaceService?.Setup(x => x.CreateNamespace(request1)).Returns(expectedNamespace1);
        var firstCallResponse = _namespaceAdapter?.CreateNamespace(request1);

        _mockNamespaceService?.Setup(x => x.CreateNamespace(request1)).Throws(new InvalidAttributeLogic("Namespace with this name already exists at this level."));
        Assert.ThrowsException<InvalidAttributeAdapter>(() => _namespaceAdapter?.CreateNamespace(request1));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowNonExistentValueAdapter_WhenBaseNamespaceDoesNotExist()
    {
        var request = new NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = Guid.NewGuid()
        };

        _mockNamespaceService?.Setup(x => x.CreateNamespace(request)).Throws(new NonExistentValueLogic("Base namespace does not exist."));

        Assert.ThrowsException<NonExistentValueAdapter>(() => _namespaceAdapter?.CreateNamespace(request));
    }
}

using Adapter;
using Domain;
using Domain.Exceptions;
using IAdapter.Exceptions;
using IBusinessLogic;
using IBusinessLogic.Exceptions;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class NamespaceAdapterTest
{
    private Mock<INamespaceService>? _mockNamespaceService;
    private NamespaceAdapter? _namespaceAdapter;
    private Mock<ISimClassService>? _simClassService;

    [TestInitialize]
    public void Setup()
    {
        _mockNamespaceService = new Mock<INamespaceService>();
        _simClassService = new Mock<ISimClassService>();
        _namespaceAdapter = new NamespaceAdapter(_mockNamespaceService.Object, _simClassService.Object);
    }

    [TestMethod]
    public void GetNamespaceById_ShouldReturnNamespaceResponse_WhenIdIsValid()
    {
        var namespaceId = Guid.NewGuid();
        var class1Id = Guid.NewGuid();
        var class2Id = Guid.NewGuid();

        var expectedNamespace = new SimNamespace
        {
            Id = namespaceId,
            Name = "TestNamespace",
            BaseNamespaceId = null,
        };
        var namespaceClasses = new List<SimClass>
        {
        new SimClass { Id = class1Id, Name = "TestClass1", NamespaceId = namespaceId, Namespace = expectedNamespace },
        new SimClass { Id = class2Id, Name = "TestClass2", NamespaceId = namespaceId, Namespace = expectedNamespace },
        };

        expectedNamespace.Elements = namespaceClasses;

        _mockNamespaceService!.Setup(x => x.GetNamespaceById(namespaceId)).Returns(expectedNamespace);
        _simClassService!.Setup(x => x.GetClassesOfNamespaces(namespaceId)).Returns(expectedNamespace.Elements);

        var response = _namespaceAdapter!.GetNamespaceById(namespaceId);

        Assert.IsNotNull(response);
        Assert.AreEqual(expectedNamespace.Id, response.Id);
        Assert.AreEqual(expectedNamespace.Name, response.Name);
        Assert.IsNull(response.BaseNamespaceId);

        Assert.IsNotNull(response.Elements);
        Assert.AreEqual(2, response.Elements.Count);
        Assert.AreEqual(class1Id, response.Elements[0].Id);
        Assert.AreEqual("TestClass1", response.Elements[0].Name);
        Assert.AreEqual(class2Id, response.Elements[1].Id);
        Assert.AreEqual("TestClass2", response.Elements[1].Name);
    }

    [TestMethod]
    public void GetNamespaceById_ShouldThrowNonExistentValueAdapter_WhenNamespaceDoesNotExist()
    {
        var namespaceId = Guid.NewGuid();
        _mockNamespaceService?.Setup(x => x.GetNamespaceById(namespaceId)).Throws(new NonExistentValueLogic("Namespace does not exist."));

        Assert.ThrowsException<NonExistentValueAdapter>(() => _namespaceAdapter?.GetNamespaceById(namespaceId));
    }

    [TestMethod]
    public void GetAllNamespaces_ShouldReturnListOfNamespaceResponses()
    {
        var namespaceId1 = Guid.NewGuid();
        var namespaceId2 = Guid.NewGuid();
        var namespaceId3 = Guid.NewGuid();
        var namespaces = new List<SimNamespace>
        {
            new SimNamespace { Id = namespaceId1, Name = "Namespace1", BaseNamespaceId = null, Elements = [] },
            new SimNamespace { Id = namespaceId2, Name = "Namespace2", BaseNamespaceId = namespaceId1, Elements = [] },
            new SimNamespace { Id = namespaceId3, Name = "Namespace3", BaseNamespaceId = null, Elements = [] },
        };
        _mockNamespaceService?.Setup(x => x.GetAllNamespaces()).Returns(namespaces);

        _mockNamespaceService?.Setup(x => x.GetNamespaceById(namespaceId1))
            .Returns(namespaces[0]);

        foreach(var ns in namespaces)
        {
            _simClassService?.Setup(x => x.GetClassesOfNamespaces(ns.Id)).Returns([]);
        }

        var response = _namespaceAdapter?.GetAllNamespaces();

        Assert.IsNotNull(response);
        Assert.AreEqual(namespaces.Count, response.Count);

        for(var i = 0; i < namespaces.Count; i++)
        {
            Assert.AreEqual(namespaces[i].Id, response[i].Id);
            Assert.AreEqual(namespaces[i].Name, response[i].Name);
            Assert.AreEqual(namespaces[i].BaseNamespaceId, response[i].BaseNamespaceId);
            Assert.AreEqual(0, response[i].Elements.Count);
        }
    }

    public void GetAllNamespaces_ShouldReturnListOfNamespaceResponses_NamespaceContainsClassesCase()
    {
        var namespaceId1 = Guid.NewGuid();
        var namespaceClasses = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "TestClass1", NamespaceId = namespaceId1 },
            new SimClass { Id = Guid.NewGuid(), Name = "TestClass2", NamespaceId = namespaceId1 },
        };
        var namespaces = new List<SimNamespace>
        {
            new SimNamespace { Id = namespaceId1, Name = "Namespace1", BaseNamespaceId = null, Elements = namespaceClasses },
        };
        _mockNamespaceService?.Setup(x => x.GetAllNamespaces()).Returns(namespaces);

        _simClassService?.Setup(x => x.GetClassesOfNamespaces(namespaceId1)).Returns(namespaceClasses);
        var response = _namespaceAdapter?.GetAllNamespaces();

        Assert.IsNotNull(response);
        Assert.AreEqual(namespaces.Count, response.Count);

        Assert.AreEqual(namespaces[0].Id, response[0].Id);
        Assert.AreEqual(namespaces[0].Name, response[0].Name);
        Assert.AreEqual(namespaces[0].BaseNamespaceId, response[0].BaseNamespaceId);

        foreach(var element in response[0].Elements)
        {
            Assert.AreEqual(element.Id, response[0].Id);
            Assert.AreEqual(element.Name, response[0].Name);
        }
    }

    [TestMethod]
    public void GetAllNamespaces_WhenNoNamespacesExist_ShouldReturnEmptyList()
    {
        _mockNamespaceService?.Setup(x => x.GetAllNamespaces()).Returns([]);

        var response = _namespaceAdapter?.GetAllNamespaces();

        Assert.IsNotNull(response);
        Assert.AreEqual(0, response.Count);
    }

    [TestMethod]
    public void CreateNamespace_ShouldReturnCreatedNamespaceResponse_WhenInputIsValid()
    {
        var namespaceRequest = new Models.Request.NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = null
        };

        var createdNamespace = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = "TestNamespace",
            BaseNamespaceId = null
        };

        _mockNamespaceService!.Setup(x => x.CreateNamespace(It.IsAny<SimNamespace>()))
            .Returns(createdNamespace);

        var response = _namespaceAdapter!.CreateNamespace(namespaceRequest);

        Assert.IsNotNull(response);
        Assert.IsNotNull(response.NamespaceResponse);
        Assert.AreEqual(createdNamespace.Id, response.NamespaceResponse.Id);
        Assert.AreEqual(createdNamespace.Name, response.NamespaceResponse.Name);
        Assert.AreEqual(createdNamespace.BaseNamespaceId, response.NamespaceResponse.BaseNamespaceId);
        Assert.AreEqual("Namespace created successfully", response.Message);
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeLogic()
    {
        var namespaceRequest = new Models.Request.NamespaceRequest
        {
            Name = string.Empty,
            BaseNamespaceId = null
        };

        Assert.ThrowsException<InvalidAttributeDomain>(() =>
            _namespaceAdapter!.CreateNamespace(namespaceRequest));
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowNonExistentValueAdapter_WhenServiceThrowsNonExistentValueLogic()
    {
        var baseNamespaceId = Guid.NewGuid();
        var namespaceRequest = new Models.Request.NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = baseNamespaceId
        };

        _mockNamespaceService!.Setup(x => x.CreateNamespace(It.IsAny<SimNamespace>()))
            .Throws(new NonExistentValueLogic("Base namespace does not exist"));

        Assert.ThrowsException<NonExistentValueAdapter>(() =>
            _namespaceAdapter!.CreateNamespace(namespaceRequest));
    }

    [TestMethod]
    public void GetNamespaceById_ShouldThrowInvalidAttributeAdapter_WhenServiceThrowsInvalidAttributeLogic()
    {
        var namespaceId = Guid.NewGuid();
        _mockNamespaceService!.Setup(x => x.GetNamespaceById(namespaceId))
            .Throws(new InvalidAttributeLogic("Invalid namespace ID"));

        Assert.ThrowsException<InvalidAttributeAdapter>(() =>
            _namespaceAdapter!.GetNamespaceById(namespaceId));
    }
}

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
    private Mock<ISimClassService>? _simClassService;

    [TestInitialize]
    public void Setup()
    {
        _mockNamespaceService = new Mock<INamespaceService>();
        _simClassService = new Mock<ISimClassService>();
        _namespaceAdapter = new NamespaceAdapter(_mockNamespaceService.Object, _simClassService.Object);
    }

    [TestMethod]
    public void CreateNamespace_ShouldReturnNamespaceResponse_WhenRequestIsValid()
    {
        var baseNamespace = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = "BaseNamespace_Test",
            BaseNamespaceId = null,
            Elements = [],
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
            Elements = [],
        };
        _mockNamespaceService?.Setup(x => x.CreateNamespace(request)).Returns(expectedNamespace);
        _mockNamespaceService?.Setup(x => x.GetNamespaceById(baseNamespace.Id)).Returns(baseNamespace);

        var response = _namespaceAdapter?.CreateNamespace(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(expectedNamespace.Id, response.Id);
        Assert.AreEqual(expectedNamespace.Name, response.Name);
        Assert.AreEqual(expectedNamespace.BaseNamespaceId, response.BaseNamespaceId);
        Assert.AreEqual(0, response.Elements.Count);
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
            Elements = []
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

    [TestMethod]
    public void GetNamespaceById_ShouldReturnNamespaceResponse_WhenIdIsValid()
    {
        var namespaceId = Guid.NewGuid();
        var namespaceClasses = new List<SimClass>
        {
            new SimClass { Id = Guid.NewGuid(), Name = "TestClass1", NamespaceId = namespaceId },
            new SimClass { Id = Guid.NewGuid(), Name = "TestClass2", NamespaceId = namespaceId }
        };
        var expectedNamespace = new SimNamespace
        {
            Id = namespaceId,
            Name = "TestNamespace",
            BaseNamespaceId = null,
            Elements = namespaceClasses
        };
        _mockNamespaceService?.Setup(x => x.GetNamespaceById(namespaceId)).Returns(expectedNamespace);
        _simClassService?.Setup(x => x.GetClassesOfNamespaces(namespaceId)).Returns(expectedNamespace.Elements);

        var response = _namespaceAdapter?.GetNamespaceById(namespaceId);

        Assert.IsNotNull(response);
        Assert.AreEqual(expectedNamespace.Id, response.Id);
        Assert.AreEqual(expectedNamespace.Name, response.Name);
        Assert.IsNull(response.BaseNamespaceId);
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
            new SimNamespace { Id = namespaceId3, Name = "Namespace3", BaseNamespaceId = null, Elements = [] }
        };
        _mockNamespaceService?.Setup(x => x.GetAllNamespaces()).Returns(namespaces);

        _mockNamespaceService?.Setup(x => x.GetNamespaceById(namespaceId1))
            .Returns(namespaces[0]);

        // Mock para clases vacías
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
            new SimClass { Id = Guid.NewGuid(), Name = "TestClass2", NamespaceId = namespaceId1 }
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
}

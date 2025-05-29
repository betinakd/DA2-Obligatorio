using Adapter;
using Domain;
using IBusinessLogic;

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
}

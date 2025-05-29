using Adapter;
using Domain;
using IBusinessLogic;
using Models.Request;
using Models.Response;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class NamespaceAdapterTest
{
    private Mock<INamespaceService>? _mockNamespaceService;
    private NamespaceAdapter? _namespaceAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockNamespaceService = new Mock<INamespaceService>(MockBehavior.Loose);
        _namespaceAdapter = new NamespaceAdapter(_mockNamespaceService.Object);
    }

    [TestMethod]
    public void CreateNamespace_ShouldReturnNamespaceResponse_WhenRequestIsValid()
    {
        var request = new Models.Request.NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = Guid.NewGuid()
        };
        var expectedNamespace = new SimNamespace
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BaseNamespaceId = request.BaseNamespaceId
        };
        _mockNamespaceService.Setup(x => x.CreateNamespace(request)).Returns(expectedNamespace);

        var response = _namespaceAdapter.CreateNamespace(request);

        Assert.IsNotNull(response);
        Assert.AreEqual(expectedNamespace.Id, response.Id);
        Assert.AreEqual(expectedNamespace.Name, response.Name);
        Assert.AreEqual(expectedNamespace.BaseNamespaceId, response.BaseNamespaceId);
    }
}

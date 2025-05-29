using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;
using Moq;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class NamespaceControllerTest
{
    private Mock<INamespaceAdapter>? _mockNamespaceAdapter;
    private NamespaceController? _namespaceController;

    [TestInitialize]
    public void Setup()
    {
        _mockNamespaceAdapter = new Mock<INamespaceAdapter>();
        _namespaceController = new NamespaceController(_mockNamespaceAdapter.Object);
    }

    [TestMethod]
    public void ExecuteMethod_ShouldReturnCreated_WhenRequestIsValid()
    {
        var request = new NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = Guid.NewGuid()
        };
        var response = new NamespaceResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BaseNamespaceId = request.BaseNamespaceId,
            Classes = [],
            Interfaces = []
        };

        _mockNamespaceAdapter.Setup(x => x.CreateNamespace(request)).Returns(response);

        var result = _namespaceController.CreateNamespace(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("GetNamespaceById", result.Location);
        Assert.AreEqual(response, result.Value);
    }
}

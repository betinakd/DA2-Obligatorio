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

    [TestMethod]
    public void AddClassInNamespace_ShouldReturnOk_WhenRequestIsValid()
    {
        var id = Guid.NewGuid();
        var request = new NamespaceElementAdd_Request { ClassId = Guid.Parse("11111111-1111-1111-1111-111111111111") };
        _mockNamespaceAdapter.Setup(x => x.AddClassInNamespace(id, request)).Returns("Class added successfully");

        var result = _namespaceController.AddClassInNamespace(id, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("Class added successfully", result.Value);
    }

    [TestMethod]
    public void AddInterfaceInNamespace_ShouldReturnOk_WhenRequestIsValid()
    {
        var id = Guid.NewGuid();
        var request = new NamespaceElementAdd_Request
        {
            InterfaceId = Guid.NewGuid(),
        };
        _mockNamespaceAdapter.Setup(x => x.AddInterfaceInNamespace(id, request)).Returns("Interface added successfully");

        var result = _namespaceController.AddInterfaceInNamespace(id, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("Interface added successfully", result.Value);
    }
}

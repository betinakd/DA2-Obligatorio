using Adapter.Exceptions;
using FluentAssertions;
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
    public void CreateNamespace_ShouldReturnCreated_WhenRequestIsValid()
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
            Elements = []
        };

        _mockNamespaceAdapter.Setup(x => x.CreateNamespace(request)).Returns(response);

        var result = _namespaceController.CreateNamespace(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("GetNamespaceById", result.Location);
        Assert.AreEqual(response, result.Value);
    }

    [TestMethod]
    public void GetNamespaceById_ShouldReturnOk_WhenNamespaceExists()
    {
        var namespaceId = Guid.NewGuid();
        var response = new NamespaceResponse
        {
            Id = namespaceId,
            Name = "TestNamespace",
            BaseNamespaceId = null,
            Elements = []
        };

        _mockNamespaceAdapter.Setup(x => x.GetNamespaceById(namespaceId)).Returns(response);

        var result = _namespaceController.GetNamespaceById(namespaceId) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(response, result.Value);
    }

    [TestMethod]
    public void GetNamespaceById_ShouldReturnNotFound_WhenNamespaceDoesNotExist()
    {
        var namespaceId = Guid.NewGuid();

        _mockNamespaceAdapter.Setup(x => x.GetNamespaceById(namespaceId)).Throws(new NonExistentValueAdapter($"Namespace with {namespaceId} ID name does not exist."));
        Action act = () => _namespaceController.GetNamespaceById(namespaceId);

        act.Should().Throw<NonExistentValueAdapter>().WithMessage($"Namespace with {namespaceId} ID name does not exist.");
    }

    [TestMethod]
    public void GetNamespaceById_ShouldReturnBadRequest_WhenIdIsNull()
    {
        _mockNamespaceAdapter.Setup(x => x.GetNamespaceById(Guid.Empty)).Throws(new InvalidAttributeAdapter("Namespace can't be empty."));
        Action act = () => _namespaceController.GetNamespaceById(Guid.Empty);
        act.Should().Throw<InvalidAttributeAdapter>().WithMessage("Namespace can't be empty.");
    }

    [TestMethod]
    public void CreateNamespace_ShouldReturnBadRequest_NameIsEmpty()
    {
        var request = new NamespaceRequest
        {
            Name = null,
            BaseNamespaceId = null
        };

        _mockNamespaceAdapter.Setup(x => x.CreateNamespace(request)).Throws(new InvalidAttributeAdapter("Namespace name can't be empty."));

        Action act = () => _namespaceController.CreateNamespace(request);
        act.Should().Throw<InvalidAttributeAdapter>().WithMessage("Namespace name can't be empty.");
    }

    [TestMethod]
    public void CreateNamespace_ShouldThrowException_WhenBaseNamespaceIdNonExists()
    {
        var request = new NamespaceRequest
        {
            Name = "TestNamespace",
            BaseNamespaceId = Guid.NewGuid()
        };

        _mockNamespaceAdapter.Setup(x => x.CreateNamespace(request)).Throws(new NonExistentValueAdapter("Base namespace does not exist."));

        Action act = () => _namespaceController.CreateNamespace(request);
        act.Should().Throw<NonExistentValueAdapter>().WithMessage("Base namespace does not exist.");
    }
}

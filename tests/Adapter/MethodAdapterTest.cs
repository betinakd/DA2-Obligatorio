using Adapter;
using Adapter.Exceptions;
using Domain;
using IBussinesLogic;
using Models.Request;
using Models.Response;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class MethodAdapterTest
{
    private Mock<IMethodService>? _mockMethodService;
    private MethodAdapter? _methodAdapter;

    [TestInitialize]
    public void Initialize()
    {
        _mockMethodService = new Mock<IMethodService>(MockBehavior.Strict);
        _methodAdapter = new MethodAdapter(_mockMethodService.Object);
    }

    [TestMethod]
    public void AddMethodParameter_ShouldReturnMethodElementsResponse_WhenValidRequest()
    {
        var methodId = Guid.NewGuid();

        var relatedClass = new SimClass { Id = Guid.NewGuid(), Name = "ValidClass" };
        var returnType = new SimAttribute { Id = Guid.NewGuid(), Name = "typeName", RelatedClass = relatedClass };
        var simMethod = new SimMethod { Id = methodId, Name = "methodName", ReturnType = returnType };

        var methodRequest = new MethodElementsRequest
        {
            Name = "name",
            Type = "typeName"
        };

        var expectedResponse = new MethodElementsResponse
        {
            Id = methodId,
            Message = "Parameter added successfully"
        };

        _mockMethodService
            ?.Setup(service => service.AddMethodParameter(methodId, methodRequest.Name, methodRequest.Type))
            .Returns(simMethod);

        var result = _methodAdapter?.AddParameter(methodId, methodRequest);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResponse.Id, result.Id);
        Assert.AreEqual(expectedResponse.Message, result.Message);

        _mockMethodService?.Verify(service => service.AddMethodParameter(methodId, methodRequest.Name, methodRequest.Type), Times.Once);
    }

    [TestMethod]
    public void AddMethodLocalVariables_ShouldReturnMethodElementsResponse_WhenValidRequest()
    {
        var methodId = Guid.NewGuid();

        var relatedClass = new SimClass { Id = Guid.NewGuid(), Name = "ValidClass" };
        var returnType = new SimAttribute { Id = Guid.NewGuid(), Name = "typeName", RelatedClass = relatedClass };
        var simMethod = new SimMethod { Id = methodId, Name = "methodName", ReturnType = returnType };

        var methodRequest = new MethodElementsRequest
        {
            Name = "name",
            Type = "typeName"
        };

        var expectedResponse = new MethodElementsResponse
        {
            Id = methodId,
            Message = "Local variable added successfully"
        };

        _mockMethodService
            ?.Setup(service => service.AddLocalVariable(methodId, methodRequest.Name, methodRequest.Type))
            .Returns(simMethod);

        var result = _methodAdapter?.AddLocalVariable(methodId, methodRequest);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResponse.Id, result.Id);
        Assert.AreEqual(expectedResponse.Message, result.Message);

        _mockMethodService?.Verify(service => service.AddLocalVariable(methodId, methodRequest.Name, methodRequest.Type), Times.Once);
    }
}

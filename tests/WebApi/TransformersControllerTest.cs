using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Moq;
using Transformers.Abstractions;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class TransformersControllerTest
{
    private Mock<ITransformerAdapter>? _mockTransformerAdapter;
    private Mock<IExecutionAdapter>? _mockExecutionAdapter;
    private TransformersController? _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockTransformerAdapter = new Mock<ITransformerAdapter>();
        _mockExecutionAdapter = new Mock<IExecutionAdapter>();
        _controller = new TransformersController(_mockTransformerAdapter.Object, _mockExecutionAdapter.Object);
    }

    [TestMethod]
    public void GetTransformers_ReturnsOkWithTransformers()
    {
        var transformers = new List<TransformerInfo> { new TransformerInfo { Id = "id", Name = "name", ContentType = "text/plain" } };
        _mockTransformerAdapter.Setup(x => x.GetTransformers()).Returns(transformers);

        var result = _controller.GetTransformers();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(transformers, okResult.Value);
    }

    [TestMethod]
    public void ExecuteWithTransform_ReturnsOkWithResult()
    {
        var request = new MethodExecutionRequest();
        var response = new TransformedResponse();
        var validKey = new Guid("77777777-aaaa-1111-1111-111111111111");
        _mockExecutionAdapter.Setup(x => x.ExecuteMethodWithTransform(request, null)).Returns(response);

        var result = _controller.ExecuteWithTransform(request, null);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(response, okResult.Value);
    }
}

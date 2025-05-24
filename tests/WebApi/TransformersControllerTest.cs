using IAdapter;
using IBusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Moq;
using Transformers.Abstractions;
using WebApi.Controllers;

namespace Tests.WebApi;

[TestClass]
public class TransformersControllerTest
{
    private Mock<ITransformerService>? _mockTransformerService;
    private Mock<IExecutionAdapter>? _mockExecutionAdapter;
    private TransformersController? _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockTransformerService = new Mock<ITransformerService>();
        _mockExecutionAdapter = new Mock<IExecutionAdapter>();
        _controller = new TransformersController(_mockTransformerService.Object, _mockExecutionAdapter.Object);
    }

    [TestMethod]
    public void GetTransformers_ReturnsOkWithTransformers()
    {
        var transformers = new List<TransformerInfo> { new TransformerInfo { Id = "id", Name = "name", ContentType = "text/plain" } };
        _mockTransformerService.Setup(x => x.GetAvailableTransformers()).Returns(transformers);

        var result = _controller.GetTransformers();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(transformers, okResult.Value);
    }

    [TestMethod]
    public void ReloadTransformers_ReturnsOkWithMessageAndTransformers()
    {
        var transformers = new List<TransformerInfo>();
        _mockTransformerService.Setup(x => x.GetAvailableTransformers()).Returns(transformers);

        var result = _controller.ReloadTransformers();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var value = okResult.Value;
        var messageProp = value.GetType().GetProperty("message");
        var transformersProp = value.GetType().GetProperty("transformers");

        Assert.IsNotNull(messageProp);
        Assert.IsNotNull(transformersProp);

        Assert.AreEqual("Transformadores recargados correctamente", messageProp.GetValue(value));
        Assert.AreEqual(transformers, transformersProp.GetValue(value));
        _mockTransformerService.Verify(x => x.LoadTransformers(), Times.Once);
    }

    [TestMethod]
    public void ExecuteWithTransform_ReturnsOkWithResult()
    {
        var request = new MethodExecutionRequest();
        var response = new TransformedResponse();
        _mockExecutionAdapter.Setup(x => x.ExecuteMethodWithTransform(request, null)).Returns(response);

        var result = _controller.ExecuteWithTransform(request, null);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(response, okResult.Value);
    }
}

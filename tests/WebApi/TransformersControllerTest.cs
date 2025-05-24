using IAdapter;
using IBusinessLogic;
using Microsoft.AspNetCore.Mvc;
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
        _controller = new TransformersController(_mockTransformerService.Object);
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
}

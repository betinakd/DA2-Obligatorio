using IAdapter;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Moq;
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
        _controller = new TransformersController(_mockTransformerAdapter.Object);
    }

    [TestMethod]
    public void GetTransformers_ReturnsOkWithTransformers()
    {
        var transformers = new[] { "HTML", "JSON" };
        _mockTransformerAdapter!.Setup(x => x.GetTransformers()).Returns(transformers);

        var result = _controller!.GetTransformers();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        CollectionAssert.AreEqual(transformers, (string[])okResult.Value!);
    }

    [TestMethod]
    public void ExecuteWithTransform_ReturnsOkWithResult()
    {
        var request = new MethodExecutionTransformedRequest { TransformerName = "HTML" };
        var expectedResult = "<html>result</html>";
        _mockTransformerAdapter!.Setup(x => x.ExportExecution(request)).Returns(expectedResult);

        var result = _controller!.ExecuteWithTransform(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expectedResult, okResult.Value);
    }
}

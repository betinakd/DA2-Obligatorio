using Adapter;
using IAdapter;
using IBusinessLogic;
using Models.Request;
using Models.Response;
using Moq;

namespace Tests.Adapter;

[TestClass]
public class TransformerAdapterTest
{
    private Mock<ITransformerService>? _mockTransformerService;
    private Mock<IExecutionAdapter>? _mockExecutionAdapter;
    private TransformerAdapter? _adapter;

    [TestInitialize]
    public void Setup()
    {
        _mockTransformerService = new Mock<ITransformerService>();
        _mockExecutionAdapter = new Mock<IExecutionAdapter>();
        _adapter = new TransformerAdapter(_mockTransformerService.Object, _mockExecutionAdapter.Object);
    }

    [TestMethod]
    public void GetTransformers_ReturnsAvailableExporters()
    {
        var exporters = new[] { "HTML", "JSON" };
        _mockTransformerService!.Setup(x => x.GetAvailableExporters()).Returns(exporters);

        var result = _adapter!.GetTransformers();

        CollectionAssert.AreEqual(exporters, result);
    }

    [TestMethod]
    public void ExportExecution_ReturnsTransformedResult()
    {
        var request = new MethodExecutionTransformedRequest
        {
            TransformerName = "HTML",
            Execution = new MethodExecutionRequest()
        };
        var executionResult = new MethodExecutionResponse { Execution = "raw result" };
        var transformed = "<html>raw result</html>";

        _mockExecutionAdapter!
            .Setup(x => x.ExecuteMethod(request.Execution))
            .Returns(executionResult);

        _mockTransformerService!
            .Setup(x => x.ExportExecution(request.TransformerName, executionResult.Execution))
            .Returns(transformed);

        var result = _adapter!.ExportExecution(request);

        Assert.AreEqual(transformed, result);
    }
}

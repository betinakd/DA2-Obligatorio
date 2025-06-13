using Adapter;
using BusinessLogic.Exceptions;
using IAdapter;
using IAdapter.Exceptions;
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

    [TestMethod]
    public void ExportExecution_ShouldThrowInvalidExecutionAdapter_WhenNonExistentValueLogicIsThrown()
    {
        var mockTransformerService = new Mock<ITransformerService>();
        var mockExecutionAdapter = new Mock<IExecutionAdapter>();
        var adapter = new TransformerAdapter(mockTransformerService.Object, mockExecutionAdapter.Object);

        var request = new MethodExecutionTransformedRequest
        {
            TransformerName = "TestTransformer",
            Execution = new MethodExecutionRequest()
        };

        mockExecutionAdapter
            .Setup(e => e.ExecuteMethod(It.IsAny<MethodExecutionRequest>()))
            .Throws(new NonExistentValueLogic("Execution not found"));

        var ex = Assert.ThrowsException<InvalidExecutionAdapter>(() =>
            adapter.ExportExecution(request));

        Assert.AreEqual("Error during execution export: Execution not found", ex.Message);
    }
}

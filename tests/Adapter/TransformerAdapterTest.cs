using Adapter;
using IBusinessLogic;
using Moq;
using Transformers.Abstractions;

namespace Tests.Adapter;

[TestClass]
public class TransformerAdapterTest
{
    [TestMethod]
    public void GetTransformers_ShouldReturnTransformersFromService()
    {
        var expectedTransformers = new List<TransformerInfo>
            {
                new TransformerInfo { Id = "test1", Name = "Test 1", ContentType = "text/plain" },
                new TransformerInfo { Id = "test2", Name = "Test 2", ContentType = "text/html" }
            };

        var mockTransformerService = new Mock<ITransformerService>();
        mockTransformerService.Setup(s => s.GetAvailableTransformers())
            .Returns(expectedTransformers);

        var adapter = new TransformerAdapter(mockTransformerService.Object);

        var result = adapter.GetTransformers();

        Assert.IsNotNull(result);
        var resultList = result.ToList();
        Assert.AreEqual(expectedTransformers.Count, resultList.Count);

        for(var i = 0; i < expectedTransformers.Count; i++)
        {
            Assert.AreEqual(expectedTransformers[i].Id, resultList[i].Id);
            Assert.AreEqual(expectedTransformers[i].Name, resultList[i].Name);
            Assert.AreEqual(expectedTransformers[i].ContentType, resultList[i].ContentType);
        }

        mockTransformerService.Verify(s => s.GetAvailableTransformers(), Times.Once);
    }

    [TestMethod]
    public void TransformExecution_ShouldReturnTransformedResponseFromService()
    {
        var executionResult = "resultado original";
        var transformerId = "test-transformer";

        var expectedResponse = new TransformedResponse
        {
            OriginalResult = executionResult,
            TransformedResult = "resultado transformado",
            ContentType = "text/html",
            TransformerId = transformerId,
            AvailableTransformers =
                [
                    new TransformerInfo { Id = transformerId, Name = "Test", ContentType = "text/html" }
                ]
        };

        var mockTransformerService = new Mock<ITransformerService>();
        mockTransformerService.Setup(s => s.TransformExecution(executionResult, transformerId))
            .Returns(expectedResponse);

        var adapter = new TransformerAdapter(mockTransformerService.Object);

        var result = adapter.TransformExecution(executionResult, transformerId);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResponse.OriginalResult, result.OriginalResult);
        Assert.AreEqual(expectedResponse.TransformedResult, result.TransformedResult);
        Assert.AreEqual(expectedResponse.ContentType, result.ContentType);
        Assert.AreEqual(expectedResponse.TransformerId, result.TransformerId);
        Assert.IsNotNull(result.AvailableTransformers);
        Assert.AreEqual(1, result.AvailableTransformers.Count);

        mockTransformerService.Verify(s => s.TransformExecution(executionResult, transformerId), Times.Once);
    }

    [TestMethod]
    public void TransformExecution_WithoutTransformerId_ShouldUseDefaultTransformer()
    {
        var executionResult = "resultado original";
        var defaultTransformerId = "default-transformer";

        var expectedResponse = new TransformedResponse
        {
            OriginalResult = executionResult,
            TransformedResult = "resultado transformado por defecto",
            ContentType = "text/plain",
            TransformerId = defaultTransformerId
        };

        var mockTransformerService = new Mock<ITransformerService>();
        mockTransformerService.Setup(s => s.TransformExecution(executionResult, null))
            .Returns(expectedResponse);

        var adapter = new TransformerAdapter(mockTransformerService.Object);

        var result = adapter.TransformExecution(executionResult, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResponse.OriginalResult, result.OriginalResult);
        Assert.AreEqual(expectedResponse.TransformedResult, result.TransformedResult);
        Assert.AreEqual(expectedResponse.ContentType, result.ContentType);
        Assert.AreEqual(expectedResponse.TransformerId, result.TransformerId);

        mockTransformerService.Verify(s => s.TransformExecution(executionResult, null), Times.Once);
    }
}

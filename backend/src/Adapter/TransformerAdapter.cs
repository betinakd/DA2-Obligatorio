using IAdapter;
using IBusinessLogic;
using Transformers.Abstractions;

namespace Adapter;

public class TransformerAdapter(ITransformerService transformerService) : ITransformerAdapter
{
    private readonly ITransformerService _transformerService = transformerService;

    public IEnumerable<TransformerInfo> GetTransformers()
    {
        return _transformerService.GetAvailableTransformers();
    }

    public TransformedResponse TransformExecution(string executionResult, string transformerId)
    {
        return _transformerService.TransformExecution(executionResult, transformerId);
    }
}

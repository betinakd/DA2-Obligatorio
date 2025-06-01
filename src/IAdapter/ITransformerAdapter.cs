using Transformers.Abstractions;

namespace IAdapter;

public interface ITransformerAdapter
{
    IEnumerable<TransformerInfo> GetTransformers();
    TransformedResponse TransformExecution(string executionResult, string transformerId);
}

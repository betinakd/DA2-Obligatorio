using Transformers.Abstractions;

namespace IBusinessLogic;
public interface ITransformerService
{
    void LoadTransformers();
    IEnumerable<TransformerInfo> GetAvailableTransformers();
    TransformedResponse TransformExecution(string executionResult, string transformerId);
}

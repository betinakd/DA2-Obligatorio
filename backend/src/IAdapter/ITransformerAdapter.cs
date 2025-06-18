using Models.Request;

namespace IAdapter;

public interface ITransformerAdapter
{
    public string[] GetTransformers();

    string ExportExecution(MethodExecutionTransformedRequest executionResult);
}

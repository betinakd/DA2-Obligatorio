using IAdapter;
using IBusinessLogic;
using Models.Request;

namespace Adapter;

public class TransformerAdapter(ITransformerService transformerService, IExecutionAdapter executionAdapter) : ITransformerAdapter
{
    private readonly ITransformerService _transformerService = transformerService;
    private readonly IExecutionAdapter _executionAdapter = executionAdapter;
    public string[] GetTransformers()
    {
        return _transformerService.GetAvailableExporters();
    }

    public string ExportExecution(MethodExecutionTransformedRequest methodExecutionRequest)
    {
        var executionResult = _executionAdapter.ExecuteMethod(methodExecutionRequest.Execution);
        return _transformerService.ExportExecution(methodExecutionRequest.TransformerName, executionResult.Execution);
    }
}

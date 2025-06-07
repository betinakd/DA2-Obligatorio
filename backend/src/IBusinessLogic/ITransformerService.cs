namespace IBusinessLogic;

public interface ITransformerService
{
    string[] GetAvailableExporters();
    public string ExportExecution(string exportType, string executionResult);
}

using System.Text.Json;
using TransformerAbstractions;
namespace JSONExporter;

public class JsonResponseTransformer : TransformerAbstractions.IExporter
{
    public string GetName() => "JSON";

    public string ExportData(string executionResult)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(executionResult, options);
    }
}

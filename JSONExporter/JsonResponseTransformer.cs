using System.Text.Json;
using TransformerAbstractions;
namespace JSONExportercopy;

public class JsonResponseTransformer : IExporter
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

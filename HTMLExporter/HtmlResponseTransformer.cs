using TransformerAbstractions;
namespace HTMLExportercopy;

public class HtmlResponseTransformer : IExporter
{
    public string GetName() => "HTML";

    public string ExportData(string executionResult)
    {
        if(string.IsNullOrEmpty(executionResult))
        {
            return "<pre>Empty result</pre>";
        }

        var html = System.Net.WebUtility.HtmlEncode(executionResult)
            .Replace(" ", "&nbsp;")
            .Replace("\n", "<br/>");

        return $"<pre>{html}</pre>";
    }
}

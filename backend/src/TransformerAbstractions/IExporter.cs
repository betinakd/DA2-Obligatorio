namespace TransformerAbstractions;

public interface IExporter
{
    string ExportData(string source);
    string GetName();
}

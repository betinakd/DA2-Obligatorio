using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BusinessLogic.Exceptions;
using IBusinessLogic;
using TransformerAbstractions;

namespace BusinessLogic;

[ExcludeFromCodeCoverage]
public class TransformerService() : ITransformerService
{
    public string[] GetAvailableExporters()
    {
        var assemblies = LoadAssemblies();
        return [.. assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IExporter).IsAssignableFrom(t))
            .Select(t => ((IExporter)(Activator.CreateInstance(t) ?? throw new NonExistentValueLogic("Non existent transformer name."))).GetName())];
    }

    private static IEnumerable<Assembly> LoadAssemblies()
    {
        var pathExporters = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

        if(!Directory.Exists(pathExporters))
        {
            Console.WriteLine("Folder not found: " + pathExporters);
            throw new Exception("Plugins folder not found. Please ensure the path is correct." + pathExporters);
        }

        var dllFiles = Directory.GetFiles(pathExporters, "*.dll", SearchOption.TopDirectoryOnly);
        foreach(var dllPath in dllFiles)
        {
            Assembly? assembly = null;

            try
            {
                assembly = Assembly.LoadFrom(dllPath);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Load failed '{dllPath}': {ex.Message}");
            }

            if(assembly != null)
            {
                yield return assembly;
            }
        }
    }

    public string ExportExecution(string exportType, string executionResult)
    {
        var assemblies = LoadAssemblies();
        var exporterType = assemblies.SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(IExporter).IsAssignableFrom(t) && ((IExporter)(Activator.CreateInstance(t) ?? throw new NonExistentValueLogic("Non existent transformer name."))).GetName().ToLower() == exportType.ToLower());

        if(exporterType == null)
        {
            throw new NonExistentValueLogic("Non existent transformer name.");
        }

        var exporter = (IExporter)Activator.CreateInstance(exporterType) ?? throw new NonExistentValueLogic("Non existent transformer name.");
        return exporter.ExportData(executionResult);
    }
}

using System.Reflection;
using IBusinessLogic;
using Transformers.Abstractions;

namespace BusinessLogic;

public class TransformerService : ITransformerService
{
    private readonly List<IResponseTransformer> _transformers = [];
    private readonly string _pluginsPath;
    public TransformerService()
    {
        _pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "Transformers");
    }

    public IEnumerable<TransformerInfo> GetAvailableTransformers()
    {
        throw new NotImplementedException();
    }

    public IResponseTransformer GetTransformerById(string id)
    {
        throw new NotImplementedException();
    }

    public void LoadTransformers()
    {
        _transformers.Clear();
        var assemblies = LoadAssemblies().ToList();
        if(!assemblies.Contains(Assembly.GetExecutingAssembly()))
        {
            assemblies.Add(Assembly.GetExecutingAssembly());
        }

        if(Assembly.GetAssembly(typeof(TransformerService)) != Assembly.GetExecutingAssembly() &&
            Assembly.GetAssembly(typeof(TransformerService)) is Assembly serviceAssembly &&
            !assemblies.Contains(serviceAssembly))
        {
            assemblies.Add(serviceAssembly);
        }
    }

    private IEnumerable<Assembly> LoadAssemblies()
    {
        if(!Directory.Exists(_pluginsPath))
        {
            Directory.CreateDirectory(_pluginsPath);
            Console.WriteLine($"Creada carpeta de plugins: {_pluginsPath}");
            yield break;
        }

        var dllFiles = Directory.GetFiles(_pluginsPath, "*.dll", SearchOption.TopDirectoryOnly);
        Console.WriteLine($"Encontrados {dllFiles.Length} archivos DLL en {_pluginsPath}");

        foreach(var dllPath in dllFiles)
        {
            Assembly assembly = null;
            try
            {
                Console.WriteLine($"Intentando cargar: {dllPath}");
                assembly = Assembly.LoadFrom(dllPath);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al cargar el ensamblado {dllPath}: {ex.Message}");
            }

            if(assembly != null)
            {
                yield return assembly;
            }
        }
    }

    public TransformedResponse TransformExecution(string executionResult, string transformerId = null)
    {
        throw new NotImplementedException();
    }
}
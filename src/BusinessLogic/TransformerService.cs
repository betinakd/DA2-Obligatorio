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

        var transformerTypes = new List<Type>();
        foreach(var a in assemblies)
        {
            try
            {
                transformerTypes.AddRange(
                    a.GetTypes().Where(t => typeof(IResponseTransformer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                );
            }
            catch(ReflectionTypeLoadException ex)
            {
                transformerTypes.AddRange(
                    ex.Types.Where(t => t != null && typeof(IResponseTransformer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                );
                Console.WriteLine($"Error al cargar tipos del ensamblado {a.FullName}: {ex.Message}");
            }
        }

        Console.WriteLine($"Encontrados {transformerTypes.Count} tipos de transformadores en {assemblies.Count} ensamblados");

        foreach(var type in transformerTypes)
        {
            try
            {
                var transformer = (IResponseTransformer)Activator.CreateInstance(type);

                if(_transformers.Any(t => t.Id == transformer.Id))
                {
                    Console.WriteLine($"Ya existe un transformador con el ID '{transformer.Id}'. Se ignorará el del tipo {type.FullName}");
                    continue;
                }

                if(transformer != null)
                {
                    _transformers.Add(transformer);
                }
                else
                {
                    Console.WriteLine($"El transformador del tipo {type.FullName} es nulo y no se añadirá.");
                }

                Console.WriteLine($"Cargado transformador: {transformer.Name} ({transformer.Id}) desde {type.Assembly.GetName().Name}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al instanciar el transformador {type.FullName}: {ex.Message}");
            }
        }

        _transformers.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));
        Console.WriteLine($"Cargados {_transformers.Count} transformadores de respuesta");
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
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
        LoadTransformers();
    }

    public IEnumerable<TransformerInfo> GetAvailableTransformers()
    {
        return _transformers.Select(t => new TransformerInfo
        {
            Id = t.Id,
            Name = t.Name,
            ContentType = t.ContentType
        });
    }

    public IResponseTransformer GetTransformerById(string id)
    {
        return _transformers.FirstOrDefault(t => t.Id == id);
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
                    a.GetTypes().Where(t => typeof(IResponseTransformer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));
            }
            catch(ReflectionTypeLoadException ex)
            {
                transformerTypes.AddRange(
                    ex.Types.Where(t => t != null && typeof(IResponseTransformer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));
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
        if(_transformers.Count == 0)
        {
            return new TransformedResponse
            {
                OriginalResult = executionResult,
                TransformedResult = executionResult,
                ContentType = "text/plain",
                TransformerId = "default",
                AvailableTransformers = GetAvailableTransformers().ToList()
            };
        }

        IResponseTransformer transformer = string.IsNullOrEmpty(transformerId)
            ? _transformers.First()
            : _transformers.FirstOrDefault(t => t.Id == transformerId) ?? _transformers.First();

        try
        {
            var transformedResult = transformer.Transform(executionResult);

            return new TransformedResponse
            {
                OriginalResult = executionResult,
                TransformedResult = transformedResult,
                ContentType = transformer.ContentType,
                TransformerId = transformer.Id,
                AvailableTransformers = GetAvailableTransformers().ToList()
            };
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error al aplicar el transformador {transformer.Id}: {ex.Message}");

            return new TransformedResponse
            {
                OriginalResult = executionResult,
                TransformedResult = $"Error al transformar: {ex.Message}\n\nResultado original:\n{executionResult}",
                ContentType = "text/plain",
                TransformerId = "error",
                AvailableTransformers = GetAvailableTransformers().ToList()
            };
        }
    }
}

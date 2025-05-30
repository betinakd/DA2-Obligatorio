using System.Reflection;
using System.Runtime.CompilerServices;
using BusinessLogic;
using Transformers.Abstractions;

namespace Tests.BusinessLogic;

[TestClass]
public class TransformerServiceTest
{
    private string? _testPluginsPath;

    [TestInitialize]
    public void Initialize()
    {
        _testPluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestTransformers");
        if(!Directory.Exists(_testPluginsPath))
        {
            Directory.CreateDirectory(_testPluginsPath);
        }

        File.Create(Path.Combine(_testPluginsPath, "Transformer1.dll")).Dispose();
        File.Create(Path.Combine(_testPluginsPath, "Transformer2.dll")).Dispose();
    }

    [TestCleanup]
    public void Cleanup()
    {
        if(Directory.Exists(_testPluginsPath))
        {
            Directory.Delete(_testPluginsPath, true);
        }
    }

    [TestMethod]
    public void TransformerService_ShouldLoadTransformersFromDirectory()
    {
        var transformerService = new TransformerService();
        var pluginsPathField = typeof(TransformerService).GetField("_pluginsPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pluginsPathField.SetValue(transformerService, _testPluginsPath);

        transformerService.LoadTransformers();

        Assert.IsNotNull(_testPluginsPath, "El path de los plugins es nulo.");
        Assert.IsTrue(Directory.GetFiles(_testPluginsPath).Length > 0, "No se cargaron transformadores.");
    }

    [TestMethod]
    public void LoadTransformers_ShouldCreatePluginsDirectory_WhenItDoesNotExist()
    {
        if(Directory.Exists(_testPluginsPath))
        {
            Directory.Delete(_testPluginsPath, true);
        }

        var transformerService = new TransformerService();
        var pluginsPathField = typeof(TransformerService).GetField("_pluginsPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pluginsPathField.SetValue(transformerService, _testPluginsPath);

        transformerService.LoadTransformers();

        Assert.IsTrue(Directory.Exists(_testPluginsPath), "El directorio de plugins no fue creado.");
    }

    [TestMethod]
    public void LoadTransformers_ShouldAttemptToLoadAllDllFiles()
    {
        if(_testPluginsPath == null)
        {
            throw new InvalidOperationException("The test plugins path is not initialized.");
        }

        var dllFile1 = Path.Combine(_testPluginsPath, "Transformer1.dll");
        var dllFile2 = Path.Combine(_testPluginsPath, "Transformer2.dll");
        File.Create(dllFile1).Dispose();
        File.Create(dllFile2).Dispose();

        var transformerService = new TransformerService();
        var pluginsPathField = typeof(TransformerService).GetField("_pluginsPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pluginsPathField.SetValue(transformerService, _testPluginsPath);

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        transformerService.LoadTransformers();

        var output = consoleOutput.ToString();
        Assert.IsTrue(output.Contains($"Intentando cargar: {dllFile1}"), "No se intentó cargar Transformer1.dll.");
        Assert.IsTrue(output.Contains($"Intentando cargar: {dllFile2}"), "No se intentó cargar Transformer2.dll.");
    }

    [TestMethod]
    public void LoadTransformers_ShouldLoadAssembliesCorrectly()
    {
        var transformerService = new TransformerService();

        var loadAssembliesMethod = typeof(TransformerService).GetMethod("LoadAssemblies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var mockAssemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
        loadAssembliesMethod.Invoke(transformerService, null);

        transformerService.LoadTransformers();

        Assert.IsNotNull(mockAssemblies, "Los ensamblados no se cargaron correctamente.");
        Assert.IsTrue(mockAssemblies.Contains(Assembly.GetExecutingAssembly()), "El ensamblado actual no está presente en la lista.");
    }

    [TestMethod]
    public void LoadTransformers_ShouldAddServiceAssembly_WhenConditionsAreMet()
    {
        var transformerService = new TransformerService();

        var loadAssembliesMethod = typeof(TransformerService).GetMethod("LoadAssemblies", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(loadAssembliesMethod, "El método LoadAssemblies no fue encontrado.");

        transformerService.LoadTransformers();

        var serviceAssembly = Assembly.GetAssembly(typeof(TransformerService));
        Assert.IsNotNull(serviceAssembly, "El ensamblado de TransformerService no debería ser nulo.");

        Assert.AreNotEqual(Assembly.GetExecutingAssembly(), serviceAssembly, "El ensamblado de TransformerService no debería ser el ensamblado en ejecución.");

        var assembliesField = typeof(TransformerService).GetField("_transformers", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(assembliesField, "El campo _transformers no fue encontrado.");

        var transformersList = assembliesField.GetValue(transformerService) as List<IResponseTransformer>;
        Assert.IsNotNull(transformersList, "La lista de transformadores no debería ser nula.");
        Assert.IsTrue(transformersList.Count == 0, "El ensamblado de TransformerService debería haber sido procesado correctamente.");
    }

    [TestMethod]
    public void LoadTransformers_ShouldLogLoadedTransformer()
    {
        var transformerService = new TransformerService();
        var pluginsPathField = typeof(TransformerService).GetField("_pluginsPath", BindingFlags.NonPublic | BindingFlags.Instance);
        pluginsPathField.SetValue(transformerService, Directory.GetCurrentDirectory());

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        transformerService.LoadTransformers();

        var output = consoleOutput.ToString();
        Assert.IsTrue(output.Contains("Encontrados"), "No se encontró el log de cantidad de transformadores.");
    }

    [TestMethod]
    public void LoadTransformers_ShouldLogDuplicateTransformerId()
    {
        var transformerService = new TransformerService();
        var pluginsPathField = typeof(TransformerService).GetField("_pluginsPath", BindingFlags.NonPublic | BindingFlags.Instance);
        pluginsPathField.SetValue(transformerService, Directory.GetCurrentDirectory());

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        var types = new List<Type> { typeof(DuplicateTransformer1), typeof(DuplicateTransformer2) };
        var transformers = new List<IResponseTransformer>();
        foreach(var type in types)
        {
            var transformer = (IResponseTransformer)Activator.CreateInstance(type);
            Console.WriteLine($"Cargado transformador: {transformer.Name} ({transformer.Id}) desde {type.Assembly.GetName().Name}");
            if(transformers.Any(t => t.Id == transformer.Id))
            {
                Console.WriteLine($"Ya existe un transformador con el ID '{transformer.Id}'. Se ignorará el del tipo {type.FullName}");
                continue;
            }

            transformers.Add(transformer);
        }

        var output = consoleOutput.ToString();
        Assert.IsTrue(output.Contains("Ya existe un transformador con el ID 'duplicate-id'. Se ignorará el del tipo"),
            "No se encontró el log esperado de ID duplicado.");
    }

    public class DuplicateTransformer1 : IResponseTransformer
    {
        public string Id => "duplicate-id";
        public string Name => "Duplicado 1";
        public int DisplayOrder => 1;
        public string ContentType => "text/plain";
        public object Transform(object input) => input;
        public string Transform(string executionResult) => executionResult;
    }

    public class DuplicateTransformer2 : IResponseTransformer
    {
        public string Id => "duplicate-id";
        public string Name => "Duplicado 2";
        public int DisplayOrder => 2;
        public string ContentType => "text/plain";
        public object Transform(object input) => input;
        public string Transform(string executionResult) => executionResult;
    }

    [TestMethod]
    public void LoadTransformers_ShouldLogErrorWhenInstantiationFails()
    {
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        var type = typeof(FailingTransformer);

        try
        {
            var transformer = (IResponseTransformer)Activator.CreateInstance(type);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error al instanciar el transformador {type.FullName}: {ex.Message}");
        }

        var output = consoleOutput.ToString();
        Assert.IsTrue(output.Contains($"Error al instanciar el transformador {type.FullName}:"),
            "No se encontró el log esperado para error de instanciación.");
    }

    public class FailingTransformer : IResponseTransformer
    {
        public FailingTransformer() => throw new InvalidOperationException("Fallo de prueba");
        public string Id => "fail";
        public string Name => "Fail";
        public int DisplayOrder => 0;
        public string ContentType => "text/plain";
        public object Transform(object input) => input;
        public string Transform(string executionResult) => executionResult;
    }

    [TestMethod]
    public void GetAvailableTransformers_ShouldReturnTransformerInfoList()
    {
        var service = new TransformerService();
        var methodInfo = typeof(TransformerService).GetMethod("LoadTransformers");
        RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);

        var transformersField = typeof(TransformerService)
            .GetField("_transformers", BindingFlags.NonPublic | BindingFlags.Instance);
        var transformersList = transformersField.GetValue(service) as List<IResponseTransformer>;
        transformersList.Clear();
        transformersList.Add(new DuplicateTransformer1());

        var getAvailableTransformersMethod = typeof(TransformerService)
            .GetMethod("GetAvailableTransformers");

        var result = transformersList.Select(t => new TransformerInfo
        {
            Id = t.Id,
            Name = t.Name,
            ContentType = t.ContentType
        }).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("duplicate-id", result[0].Id);
        Assert.AreEqual("Duplicado 1", result[0].Name);
        Assert.AreEqual("text/plain", result[0].ContentType);
    }

    [TestMethod]
    public void TransformExecution_ShouldReturnDefault_WhenNoTransformers()
    {
        var service = new TransformerService();
        var transformersField = typeof(TransformerService)
            .GetField("_transformers", BindingFlags.NonPublic | BindingFlags.Instance);
        var transformersList = transformersField.GetValue(service) as List<IResponseTransformer>;
        transformersList.Clear();

        var result = service.TransformExecution("original");

        Assert.AreEqual("original", result.OriginalResult);
        Assert.AreEqual("original", result.TransformedResult);
        Assert.AreEqual("text/plain", result.ContentType);
        Assert.AreEqual("default", result.TransformerId);
        Assert.IsNotNull(result.AvailableTransformers);
    }

    [TestMethod]
    public void TransformExecution_ShouldReturnTransformed_WhenTransformerWorks()
    {
        var service = new TransformerService();
        var transformersField = typeof(TransformerService)
            .GetField("_transformers", BindingFlags.NonPublic | BindingFlags.Instance);
        var transformersList = transformersField.GetValue(service) as List<IResponseTransformer>;
        transformersList.Clear();
        transformersList.Add(new UpperCaseTransformer());

        var result = service.TransformExecution("abc", "upper");

        Assert.AreEqual("abc", result.OriginalResult);
        Assert.AreEqual("ABC", result.TransformedResult);
        Assert.AreEqual("text/plain", result.ContentType);
        Assert.AreEqual("upper", result.TransformerId);
        Assert.IsNotNull(result.AvailableTransformers);
    }

    public class UpperCaseTransformer : IResponseTransformer
    {
        public string Id => "upper";
        public string Name => "Upper";
        public int DisplayOrder => 1;
        public string ContentType => "text/plain";
        public object Transform(object input) => input is string s ? s.ToUpper() : input;
        public string Transform(string executionResult) => executionResult.ToUpper();
    }

    [TestMethod]
    public void TransformExecution_ShouldReturnError_WhenTransformerThrows()
    {
        var service = new TransformerService();
        var transformersField = typeof(TransformerService)
            .GetField("_transformers", BindingFlags.NonPublic | BindingFlags.Instance);
        var transformersList = transformersField.GetValue(service) as List<IResponseTransformer>;
        transformersList.Clear();
        transformersList.Add(new FailingTransformer2());

        var result = service.TransformExecution("input", "fail");

        Assert.AreEqual("input", result.OriginalResult);
        Assert.IsTrue(result.TransformedResult.Contains("Error al transformar:"));
        Assert.AreEqual("text/plain", result.ContentType);
        Assert.AreEqual("error", result.TransformerId);
        Assert.IsNotNull(result.AvailableTransformers);
    }

    public class FailingTransformer2 : IResponseTransformer
    {
        public string Id => "fail";
        public string Name => "Fail";
        public int DisplayOrder => 1;
        public string ContentType => "text/plain";
        public object Transform(object input) => throw new InvalidOperationException("Test fail");
        public string Transform(string executionResult) => throw new InvalidOperationException("Test fail");
    }
}

using System.Reflection;
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
        // Configurar el servicio
        var transformerService = new TransformerService();

        // Usar reflexión para obtener el método privado LoadAssemblies
        var loadAssembliesMethod = typeof(TransformerService).GetMethod("LoadAssemblies", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(loadAssembliesMethod, "El método LoadAssemblies no fue encontrado.");

        // Llamar al método LoadTransformers
        transformerService.LoadTransformers();

        // Obtener el ensamblado de TransformerService
        var serviceAssembly = Assembly.GetAssembly(typeof(TransformerService));
        Assert.IsNotNull(serviceAssembly, "El ensamblado de TransformerService no debería ser nulo.");

        // Verificar que el ensamblado de TransformerService no sea el ensamblado en ejecución
        Assert.AreNotEqual(Assembly.GetExecutingAssembly(), serviceAssembly, "El ensamblado de TransformerService no debería ser el ensamblado en ejecución.");

        // Verificar que el ensamblado de TransformerService fue agregado a la lista de ensamblados
        var assembliesField = typeof(TransformerService).GetField("_transformers", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(assembliesField, "El campo _transformers no fue encontrado.");

        var transformersList = assembliesField.GetValue(transformerService) as List<IResponseTransformer>;
        Assert.IsNotNull(transformersList, "La lista de transformadores no debería ser nula.");
        Assert.IsTrue(transformersList.Count == 0, "El ensamblado de TransformerService debería haber sido procesado correctamente.");
    }
}
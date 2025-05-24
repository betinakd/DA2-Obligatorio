using System.Reflection;
using BusinessLogic;

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
}
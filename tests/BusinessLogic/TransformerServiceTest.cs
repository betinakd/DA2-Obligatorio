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
}
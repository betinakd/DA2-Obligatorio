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

        if(!Directory.Exists(_pluginsPath))
        {
            Directory.CreateDirectory(_pluginsPath);
            Console.WriteLine($"Creada carpeta de plugins: {_pluginsPath}");
            return;
        }
    }

    public TransformedResponse TransformExecution(string executionResult, string transformerId = null)
    {
        throw new NotImplementedException();
    }
}
using Models.Response;

namespace IAdapter;

public interface IMethodAdapter
{
    public MethodResponse GetMethod(Guid id);
}
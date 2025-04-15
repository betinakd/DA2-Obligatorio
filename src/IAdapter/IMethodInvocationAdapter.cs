using Models.Response;

namespace IAdapter;

public interface IMethodInvocationAdapter
{
    public InvocationResponse GetInvocation(Guid id);
}

using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IMethodAdapter
{
    public MethodResponse GetMethod(Guid id);
    public CreatedMethodResponse CreateMethod(MethodRequest method);
    public void DeleteMethod(Guid id);
}
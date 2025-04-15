using Models.Request;
using Models.Response;

namespace IAdapter;

public interface IMethodAdapter
{
    public MethodResponse GetMethod(Guid id);
    public CreatedMethodResponse CreateMethod(Guid idClass, MethodRequest method);
    public void DeleteMethod(Guid id);
    public MethodElementsResponse UpdateMethod(Guid id, MethodRequest method);
    public MethodElementsResponse AddParameter(Guid id, MethodElementsRequest method);
}

using Adapter.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class MethodAdapter(IMethodService methodService)
    : IMethodAdapter
{
    public MethodResponse GetMethod(Guid id)
    {
        throw new NotImplementedException();
    }

    public CreatedMethodResponse CreateMethod(Guid idClass, MethodRequest method)
    {
        throw new NotImplementedException();
    }

    public void DeleteMethod(Guid id)
    {
        throw new NotImplementedException();
    }

    public MethodElementsResponse AddParameter(Guid id, MethodElementsRequest method)
    {
        if(method.Name == null || method.Type == null)
        {
            throw new InvalidAttribute("Parameter information cant be empty");
        }

        var methodParameter = methodService.AddMethodParameter(id, method.Name, method.Type);
        var response = new MethodElementsResponse
        {
            Id = methodParameter.Id,
            Message = "Parameter added successfully"
        };
        return response;
    }
}

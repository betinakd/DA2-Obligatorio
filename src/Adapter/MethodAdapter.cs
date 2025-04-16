using Adapter.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class MethodAdapter(IMethodService methodService)
    : IMethodAdapter
{
    private readonly IMethodService _methodService = methodService;
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

    public VariableResponse GetVariable(Guid id)
    {
        throw new NotImplementedException();
    }

    public CreatedVariableResponse CreateVariable(Guid idMethod, VariableRequest variable)
    {
        if (string.IsNullOrWhiteSpace(variable.Name) || string.IsNullOrWhiteSpace(variable.Type))
        {
            throw new InvalidAttribute("Local variable information cant be empty");
        }

        var newAttribute = _methodService.AddLocalVariable(idMethod, variable.Name, variable.Type);
        var response = new CreatedVariableResponse
        {
            Message = "Local variable added successfully",
            Variable = new VariableResponse
            {
                Id = newAttribute.Id,
                Name = newAttribute.Name,
                MethodId = idMethod,
                Type = newAttribute.RelatedClass.Name
            }
        };
        return response;
    }

    public ParameterResponse GetParameter(Guid id)
    {
        throw new NotImplementedException();
    }

    public CreatedParameterResponse CreateParameter(Guid idMethod, ParameterRequest parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter.Name) || string.IsNullOrWhiteSpace(parameter.Type))
        {
            throw new InvalidAttribute("Parameter information cant be empty");
        }

        var methodParameter = _methodService.AddMethodParameter(idMethod, parameter.Name, parameter.Type);
        var response = new CreatedParameterResponse
        {
            Message = "Parameter added successfully",
            Parameter = new ParameterResponse
            {
                Name = parameter.Name, MethodId = idMethod, Type = parameter.Type
            }
        };
        return response;
    }

    public CreatedInvocationResponse CreateInvocation(Guid idMethod, InvocationRequest method)
    {
        throw new NotImplementedException();
    }

    public InvocationResponse GetInvocation(Guid id)
    {
        throw new NotImplementedException();
    }
}

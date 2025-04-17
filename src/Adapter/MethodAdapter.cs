using Adapter.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class MethodAdapter(IMethodService methodService, ISimClassService simClassService)
    : IMethodAdapter
{
    private readonly IMethodService _methodService = methodService;
    private readonly ISimClassService _simClassService = simClassService;

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
        try
        {
            var type = _simClassService.GetSimClassById(variable.ClassTypeId);
            var method = _methodService.GetMethodById(idMethod);
            var localVariable = new LocalVariable()
            {
                Id = Guid.NewGuid(),
                Name = variable.Name,
                Type = type,
                RelatedMethod = method,
            };
            var newAttribute = _methodService.AddLocalVariable(idMethod, localVariable);
            var response = new CreatedVariableResponse
            {
                Message = "Variable created successfully",
                Variable = new VariableResponse()
                {
                    Id = newAttribute.Id,
                    Name = newAttribute.Name,
                    MethodId = idMethod,
                    ClassTypeId = newAttribute.RelatedClass.Id
                }
            };
            return response;
        }
        catch(SimClassInvalidAttribute ex)
        {
            throw new InvalidAttribute(ex.Message);
        }
    }

    public ParameterResponse GetParameter(Guid id)
    {
        throw new NotImplementedException();
    }

    public CreatedParameterResponse CreateParameter(Guid idMethod, ParameterRequest parameter)
    {
        if(string.IsNullOrWhiteSpace(parameter.Name) || string.IsNullOrWhiteSpace(parameter.Type))
        {
            throw new InvalidAttribute("Parameter information cant be empty");
        }

        var methodParameter = _methodService.AddMethodParameter(idMethod, parameter.Name, parameter.Type);
        var response = new CreatedParameterResponse
        {
            Message = "Parameter added successfully",
            Parameter = new ParameterResponse
            {
                Name = methodParameter.Name,
                MethodId = idMethod,
                Type = parameter.Type
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

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
        try
        {
            var variable = _methodService.GetVariableById(id);
            return new VariableResponse
            {
                Id = variable.Id,
                Name = variable.Name,
                MethodId = variable.RelatedMethod.Id,
                ClassTypeId = variable.Type.Id
            };
        }
        catch(SimClassInvalidAttribute)
        {
            throw new InvalidOperationException("Invalid variable ID.");
        }
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
        try
        {
            var parameter = _methodService.GetParameterById(id);
            return new ParameterResponse
            {
                Id = parameter.Id,
                Name = parameter.Name,
                MethodId = parameter.RelatedMethod.Id,
                ClassTypeId = parameter.Type.Id
            };
        }
        catch(SimClassInvalidAttribute)
        {
            throw new InvalidOperationException("Invalid parameter ID.");
        }
    }

    public CreatedParameterResponse CreateParameter(Guid idMethod, ParameterRequest parameter)
    {
        try
        {
            var type = _simClassService.GetSimClassById(parameter.ClassTypeId);
            var method = _methodService.GetMethodById(idMethod);
            var parameterMethod = new Parameter()
            {
                Id = Guid.NewGuid(),
                Name = parameter.Name,
                Type = type,
                RelatedMethod = method,
            };
            var newAttribute = _methodService.AddMethodParameter(idMethod, parameterMethod);
            var response = new CreatedParameterResponse
            {
                Message = "Parameter created successfully",
                Parameter = new ParameterResponse()
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

    public CreatedInvocationResponse CreateInvocation(Guid idMethod, InvocationRequest method)
    {
        throw new NotImplementedException();
    }

    public InvocationResponse GetInvocation(Guid id)
    {
        throw new NotImplementedException();
    }
}

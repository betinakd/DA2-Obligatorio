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
        try
        {
            var method = _methodService.GetMethodById(id);
            return new MethodResponse
            {
                Id = method.Id,
                Name = method.Name,
                IdClassOwner = method.RelatedClass.Id,
                Privacity = EnumMapper.MapToModelPrivacity(method.Privacity),
                Accesibility = EnumMapper.MapToModelAccesibility(method.Accesibility),
                ReturnTypeId = method.ReturnType.Id
            };
        }
        catch(SimClassInvalidAttribute)
        {
            throw new InvalidOperationException("Invalid method ID.");
        }
    }

    public CreatedMethodResponse CreateMethod(Guid idClass, MethodRequest method)
    {
        try
        {
            var classOwner = _simClassService.GetSimClassById(idClass);
            var returnType = _simClassService.GetSimClassById(method.ReturnTypeId);

            var newMethod = new SimMethod
            {
                Id = Guid.NewGuid(),
                Name = method.Name,
                RelatedClass = classOwner,
                Privacity = EnumMapper.MapToDomainPrivacity(method.Privacity),
                Accesibility = EnumMapper.MapToDomainAccesibility(method.Accesibility),
                ReturnType = returnType
            };

            var createdMethod = _methodService.AddMethod(idClass, newMethod);

            return new CreatedMethodResponse
            {
                Message = "Method created successfully",
                MethodResponse = new MethodResponse
                {
                    Id = createdMethod.Id,
                    Name = createdMethod.Name,
                    IdClassOwner = createdMethod.RelatedClass.Id,
                    Privacity = EnumMapper.MapToModelPrivacity(createdMethod.Privacity),
                    Accesibility = EnumMapper.MapToModelAccesibility(createdMethod.Accesibility),
                    ReturnTypeId = createdMethod.ReturnType.Id
                }
            };
        }
        catch(SimClassInvalidAttribute ex)
        {
            throw new InvalidAttribute(ex.Message);
        }
    }

    public void DeleteMethod(Guid id)
    {
        try
        {
            _methodService.DeleteMethod(id);
        }
        catch(SimClassInvalidAttribute)
        {
            throw new InvalidOperationException("Invalid method ID.");
        }
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

    public CreatedInvocationResponse CreateInvocation(Guid idMethod, InvocationRequest invocation)
    {
        var method = _methodService.GetMethodById(idMethod);

        var newInvocation = new Invocation
        {
            Id = Guid.NewGuid(),
            MethodName = method.Name,
            ReferenceId = invocation.IdReference,
            RelatedMethod = method
        };
        var parametersResponses = new List<ParameterResponse>();
        foreach(var parameter in invocation.Parameters)
        {
            var newParameter = new Parameter()
            {
                Id = Guid.NewGuid(),
                Name = parameter.Name,
                Type = _simClassService.GetSimClassById(parameter.ClassTypeId),
                RelatedMethod = _methodService.GetMethodById(idMethod)
            };

            var newParameterResponse = new ParameterResponse()
            {
                Id = newParameter.Id,
                Name = newParameter.Name,
                MethodId = idMethod,
                ClassTypeId = newParameter.Type.Id
            };

            newInvocation.Parameters.Add(newParameter);
            parametersResponses.Add(newParameterResponse);
        }

        _ = _methodService.AddInvocation(idMethod, newInvocation);

        return new CreatedInvocationResponse
        {
            Message = "Invocation created successfully",
            InvocationResponse = new InvocationResponse
            {
                Id = newInvocation.Id,
                IdReference = newInvocation.ReferenceId,
                MethodName = newInvocation.MethodName,
                Parameters = parametersResponses,
            }
        };
    }

    public InvocationResponse GetInvocation(Guid id)
    {
        throw new NotImplementedException();
    }
}

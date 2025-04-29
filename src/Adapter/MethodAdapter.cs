using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Enums;
using Models.Request;
using Models.Response;

namespace Adapter;

public class MethodAdapter(IMethodService methodService, ISimClassService simClassService, ISimAttributeService simAttributeService, IExecutionService executionService)
    : IMethodAdapter
{
    private readonly IExecutionService _executionService = executionService;
    private readonly IMethodService _methodService = methodService;
    private readonly ISimClassService _simClassService = simClassService;
    private readonly ISimAttributeService _simAttributeService = simAttributeService;

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
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
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
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    public void DeleteMethod(Guid id)
    {
        try
        {
            _methodService.DeleteMethod(id);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
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
        catch(NonExistentValueLogic)
        {
            throw new NonExistentValueAdapter("Invalid variable ID.");
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
                    MethodId = newAttribute.RelatedMethod.Id,
                    ClassTypeId = newAttribute.Type.Id
                }
            };
            return response;
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
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
        catch(InvalidAttributeDomain)
        {
            throw new InvalidAttributeAdapter("Invalid parameter ID.");
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
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
                    MethodId = newAttribute.RelatedMethod.Id,
                    ClassTypeId = newAttribute.Type.Id
                }
            };
            return response;
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    public CreatedInvocationResponse CreateInvocation(Guid idMethod, InvocationRequest invocation)
    {
        try
        {
            var method = _methodService.GetMethodById(idMethod);
            Reference reference = null;
            var signature = new Signature()
            {
                Name = invocation.MethodName,
                Parameters = []
            };

            var parametersResponses = new List<ParameterResponse>();

            foreach(var parameter in invocation.Parameters)
            {
                var newParameter = new ParameterSignature()
                {
                    Name = parameter.Name,
                    Type = _simClassService.GetSimClassById(parameter.ClassTypeId),
                };

                var newParameterResponse = new ParameterResponse()
                {
                    Name = newParameter.Name,
                    ClassTypeId = newParameter.Type.Id
                };

                signature.Parameters.Add(newParameter);
                parametersResponses.Add(newParameterResponse);
            }

            switch(invocation.TypeReference)
            {
                case TypeReference.This:
                    reference = new ReferenceThis() { Reference = _simClassService.GetSimClassById(invocation.IdReference) };
                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.Base:
                    reference = new ReferenceBase() { Reference = _simClassService.GetSimClassById(invocation.IdReference) };
                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.Attribute:
                    var attribute = _simAttributeService.GetSimAttribute(invocation.IdReference);
                    reference = new ReferenceAttribute() { Reference = attribute };
                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.Parameter:
                    var parameter = _methodService.GetParameterById(invocation.IdReference);
                    reference = new ReferenceParameter() { Reference = parameter };
                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.LocalVariable:
                    var variable = _methodService.GetVariableById(invocation.IdReference);
                    reference = new ReferenceVariable() { Reference = variable };
                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                default:
                    throw new InvalidAttributeAdapter($"Unsupported type reference : {invocation.TypeReference}.");
            }

            var newInvocation = new Invocation
            {
                Id = Guid.NewGuid(),
                Reference = reference,
                ReferenceId = reference.Id,
                Signature = signature,
                RelatedMethod = method,
                RelatedMethodId = method.Id,
                SignatureId = signature.Id
            };

            _methodService.AddInvocation(idMethod, newInvocation);

            return new CreatedInvocationResponse
            {
                Message = "Invocation created successfully",
                InvocationResponse = new InvocationResponse
                {
                    Id = newInvocation.Id,
                    IdReference = newInvocation.Reference.Id,
                    MethodName = newInvocation.Signature.Name,
                    Parameters = parametersResponses,
                }
            };
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter("Error creating invocation: " + ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter("Error creating invocation: " + ex.Message);
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter("Error creating invocation: Invalid attribute domain." + ex.Message);
        }
    }

    public InvocationResponse GetInvocation(Guid id)
    {
        try
        {
            var invocation = _methodService.GetInvocationById(id);

            var parameters = new List<ParameterResponse>();
            if(invocation.Signature.Parameters != null)
            {
                foreach(var parameter in invocation.Signature.Parameters)
                {
                    parameters.Add(new ParameterResponse
                    {
                        Id = parameter.Id,
                        Name = parameter.Name,
                        ClassTypeId = parameter.TypeId
                    });
                }
            }

            return new InvocationResponse
            {
                Id = invocation.Id,
                IdReference = invocation.Reference.GetSimClass().Id,
                MethodName = invocation.Signature.Name,
                Parameters = parameters
            };
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }
}

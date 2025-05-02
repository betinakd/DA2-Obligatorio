using Adapter.Exceptions;
using Adapter.Helpers;
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
            return MethodResponseMapper.MapToMethodResponse(method);
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
                RelatedClassId = classOwner.Id,
                Privacity = EnumMapper.MapToDomainPrivacity(method.Privacity),
                Accesibility = EnumMapper.MapToDomainAccesibility(method.Accesibility),
                ReturnType = returnType,
                ReturnTypeId = returnType.Id
            };

            var createdMethod = _methodService.AddMethod(idClass, newMethod);

            return new CreatedMethodResponse
            {
                Message = "Method created successfully",
                MethodResponse = MethodResponseMapper.MapToMethodResponse(createdMethod)
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
            return VariableResponseMapper.MapToVariableResponse(variable);
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
                RelatedMethodId = idMethod
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
            return ParameterResponseMapper.MapToParameterResponse(parameter);
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
                RelatedMethodId = idMethod,
                TypeId = type.Id
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
                var type = _simClassService.GetSimClassById(parameter.ClassTypeId);
                var newParameter = new ParameterSignature()
                {
                    Name = parameter.Name,
                    Type = type,
                    TypeId = type.Id,
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
                    if(method.RelatedClassId != reference.GetSimClass().Id)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.Base:
                    var simClass = _simClassService.GetSimClassById(invocation.IdReference);
                    reference = new ReferenceBase() { Reference = simClass };
                    if(method.RelatedClassId != simClass.Id)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.Attribute:
                    var attribute = _simAttributeService.GetSimAttribute(invocation.IdReference);
                    _executionService.ClassInheritAttribute(method.RelatedClassId, attribute.Id);
                    reference = new ReferenceAttribute() { Reference = attribute };
                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.Parameter:
                    var parameter = _methodService.GetParameterById(invocation.IdReference);
                    reference = new ReferenceParameter() { Reference = parameter };
                    if(idMethod != parameter.RelatedMethodId)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    _executionService.ValidateMethodExistsInClass(reference.GetSimClass(), signature);
                    break;

                case TypeReference.LocalVariable:
                    var variable = _methodService.GetVariableById(invocation.IdReference);
                    reference = new ReferenceVariable() { Reference = variable };
                    if(idMethod != variable.RelatedMethodId)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

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
                InvocationResponse = InvocationResponseMapper.MapToInvocationResponse(newInvocation),
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
            return InvocationResponseMapper.MapToInvocationResponse(invocation);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }
}

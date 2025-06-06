using Adapter.Helpers;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IAdapter.Exceptions;
using IBusinessLogic;
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
                ReturnTypeId = returnType.Id,
                IsStatic = method.IsStatic,
                IsVirtual = method.IsVirtual,
                IsOverride = method.IsOverride,
            };

            newMethod.Validate();
            var index = 0;
            var parmeters = new List<Parameter>();
            foreach(var parameter in method.Parameters)
            {
                var type = _simClassService.GetSimClassById(parameter.ReferenceId);
                parmeters.Add(new Parameter
                {
                    Id = Guid.NewGuid(),
                    Name = parameter.Name,
                    Reference = type,
                    ReferenceId = type.Id,
                    RelatedMethod = newMethod,
                    RelatedMethodId = newMethod.Id,
                    Index = index
                });
                index++;
            }

            newMethod.Parameters = parmeters;

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

    public CreatedVariableResponse CreateVariable(Guid idMethod, VariablesRequest variable)
    {
        try
        {
            var method = _methodService.GetMethodById(idMethod);

            var type = _simClassService.GetSimClassById(variable.ReferenceId);
            var instance = _simClassService.GetSimClassById(variable.InstanceId);

            _simClassService.ValidPolymorphism(type, instance);

            var localVariable = new LocalVariable()
            {
                Id = Guid.NewGuid(),
                Name = variable.Name,
                Reference = type,
                ReferenceId = type.Id,
                RelatedMethod = method,
                RelatedMethodId = idMethod,
                Instance = instance,
                InstanceId = instance.Id
            };
            var newAttribute = _methodService.AddLocalVariable(idMethod, localVariable);
            var response = new CreatedVariableResponse
            {
                Message = "Variable created successfully",
                Variable = VariableResponseMapper.MapToVariableResponse(newAttribute)
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
            var type = _simClassService.GetSimClassById(parameter.ReferenceId);
            var method = _methodService.GetMethodById(idMethod);
            var parameterMethod = new Parameter()
            {
                Id = Guid.NewGuid(),
                Name = parameter.Name,
                Reference = type,
                RelatedMethod = method,
                RelatedMethodId = idMethod,
                ReferenceId = type.Id,
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
                    ReferenceId = newAttribute.Reference.Id
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
                Parameters = [],
                ReturnTypeId = invocation.ReturnTypeId,
            };

            var parametersResponses = new List<ParameterRequest>();
            var index = 0;
            foreach(var parameter in invocation.Parameters)
            {
                var type = _simClassService.GetSimClassById(parameter.ReferenceId);
                var instance = _simClassService.GetSimClassById(parameter.InstanceId);

                _simClassService.ValidPolymorphism(type, instance);

                var newParameter = new ParameterSignature()
                {
                    Signature = signature,
                    SignatureId = signature.Id,
                    Name = parameter.Name,
                    Reference = type,
                    ReferenceId = type.Id,
                    Instance = instance,
                    InstanceId = instance.Id,
                    Index = index
                };

                index++;

                signature.Parameters.Add(newParameter);
            }

            switch(invocation.TypeReference)
            {
                case TypeReference.This:
                    var simClass = _simClassService.GetSimClassById(invocation.ReferenceId);
                    reference = new ReferenceThis() { Reference = simClass, ReferenceId = simClass.Id };
                    if(method.RelatedClassId != reference.GetReferenceClass().Id)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    _executionService.ValidateMethodExistsInClass(reference.GetReferenceClass(), signature, false);
                    break;

                case TypeReference.Base:
                    var classBase = _simClassService.GetSimClassById(invocation.ReferenceId);
                    if(classBase.Id != method.RelatedClassId)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    reference = new ReferenceBase() { Reference = classBase, ReferenceId = classBase.Id };
                    if(classBase.BaseClass == null)
                    {
                        throw new InvalidAttributeAdapter("Base class is null and cannot be validated.");
                    }

                    _executionService.ValidateMethodExistsInClass(classBase.BaseClass, signature, true);
                    break;

                case TypeReference.Attribute:
                    var attribute = _simAttributeService.GetSimAttribute(invocation.ReferenceId);
                    _methodService.MethodInheritsAttribute(method, attribute);
                    reference = new ReferenceAttribute() { Reference = attribute, ReferenceId = attribute.Id };
                    var isNotAbstract = false;
                    _executionService.ValidateMethodExistsInClass(reference.GetReferenceClass(), signature, false);
                    break;

                case TypeReference.Parameter:
                    var parameter = _methodService.GetParameterById(invocation.ReferenceId);
                    reference = new ReferenceParameter() { Reference = parameter, ReferenceId = parameter.Id };
                    if(idMethod != parameter.RelatedMethodId)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    _executionService.ValidateMethodExistsInClass(reference.GetReferenceClass(), signature, false);
                    break;

                case TypeReference.LocalVariable:
                    var variable = _methodService.GetVariableById(invocation.ReferenceId);
                    reference = new ReferenceVariable() { Reference = variable };
                    if(idMethod != variable.RelatedMethodId)
                    {
                        throw new InvalidAttributeAdapter("Method's related class ID does not match the reference class ID.");
                    }

                    _executionService.ValidateMethodExistsInClass(reference.GetReferenceClass(), signature, false);
                    break;
                case TypeReference.StaticAttribute:
                    var staticAttribute = _simAttributeService.GetSimAttribute(invocation.ReferenceId);
                    reference = new ReferenceStaticAttribute() { Reference = staticAttribute, ReferenceId = staticAttribute.Id };
                    _methodService.ValidateStaticAttributeAccessibility(staticAttribute, idMethod);
                    _executionService.ValidateMethodExistsInClass(reference.GetReferenceClass(), signature, false);
                    break;
                case TypeReference.Static:
                    var staticClass = _simClassService.GetSimClassById(invocation.ReferenceId);
                    reference = new ReferenceStatic() { Reference = staticClass, ReferenceId = staticClass.Id };
                    _methodService.SignatureStaticExistsInClass(staticClass, idMethod, signature);
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

            newInvocation.Reference.RelatedInvocation = newInvocation;
            newInvocation.Reference.RelatedInvocationId = newInvocation.Id;

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

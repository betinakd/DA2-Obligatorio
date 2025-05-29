using Adapter.Exceptions;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Enums;
using IAdapter;
using IBusinessLogic;
using Models.Request;
using Transformers.Abstractions;

namespace Adapter;

public class ExecutionAdapter(IExecutionService executionService, ISimClassService simClassService,
        ITransformerService transformerService) : IExecutionAdapter
{
    private readonly IExecutionService _executionService = executionService;
    private readonly ISimClassService _simClassService = simClassService;
    private readonly ITransformerService _transformerService = transformerService;

    public string ExecuteMethod(MethodExecutionRequest request)
    {
        try
        {
            var parameters = new List<ParameterSignature>();
            var signature = new Signature()
            {
                Name = request.MethodName
            };

            var index = 0;
            foreach(var parameter in request.Parameters)
            {
                var typeParameter = _simClassService.GetSimClassById(parameter.ClassTypeId);
                var par = new ParameterSignature()
                {
                    Name = parameter.Name,
                    Type = typeParameter,
                    TypeId = typeParameter.Id,
                    Signature = signature,
                    SignatureId = signature.Id,
                    Index = index
                };
                index++;
                parameters.Add(par);
            }

            signature.Parameters = parameters;

            var refer = _simClassService.GetSimClassById(request.ReferenceTypeId);
            var obj = _simClassService.GetSimClassById(request.InstanceTypeId);

            if(obj.State == SimAccesibility.Abstract)
            {
                throw new InvalidExecutionAdapter("Cannot create an instance of the abstract type.");
            }

            if(!_executionService.IsReferenceBaseOfInstance(refer, obj))
            {
                throw new InvalidExecutionAdapter("Reference is not base of the instance");
            }

            var reference = new ReferenceThis()
            {
                Reference = refer
            };
            var objToCreate = new ReferenceThis()
            {
                Reference = obj
            };
            var execution = _executionService.ExecuteMethod(reference, objToCreate, signature);
            _executionService.SaveExecutionLog(refer.Name, obj.Name, execution);
            return execution;
        }
        catch(InvalidOperationLogic ex)
        {
            throw new InvalidExecutionAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    public TransformedResponse ExecuteMethodWithTransform(Guid apiKey, MethodExecutionRequest request, string transformerId = null)
    {
        if(!IsAuthorizedUser(apiKey))
        {
            throw new InvalidApikeyAdapter("API Key inválida o ausente");
        }

        var executionResult = ExecuteMethod(request);
        return _transformerService.TransformExecution(executionResult, transformerId);
    }

    public bool IsAuthorizedUser(Guid apiKey)
    {
        return _executionService.IsAuthorizedUser(apiKey);
    }
}

using BusinessLogic.Exceptions;
using Domain;
using IAdapter;
using IAdapter.Exceptions;
using IBusinessLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class ExecutionAdapter(IExecutionService executionService, ISimClassService simClassService) : IExecutionAdapter
{
    private readonly IExecutionService _executionService = executionService;
    private readonly ISimClassService _simClassService = simClassService;

    public MethodExecutionResponse ExecuteMethod(MethodExecutionRequest request)
    {
        try
        {
            var returnType = _simClassService.GetSimClassById(request.ReturnTypeId);
            var parameters = new List<ParameterSignature>();
            var signature = new Signature()
            {
                Name = request.MethodName,
                ReturnTypeId = returnType.Id,
                ReturnType = returnType,
            };

            var index = 0;
            foreach(var parameter in request.Parameters)
            {
                var typeParameter = _simClassService.GetSimClassById(parameter.ReferenceId);
                var instance = _simClassService.GetSimClassById(parameter.InstanceId);

                var par = new ParameterSignature()
                {
                    Name = parameter.Name,
                    Reference = typeParameter,
                    ReferenceId = typeParameter.Id,
                    Signature = signature,
                    SignatureId = signature.Id,
                    Instance = instance,
                    InstanceId = instance.Id,
                    Index = index
                };
                index++;
                parameters.Add(par);
            }

            signature.Parameters = parameters;

            var refer = _simClassService.GetSimClassById(request.ReferenceTypeId);
            var obj = _simClassService.GetSimClassById(request.InstanceTypeId);

            var reference = new ReferenceThis()
            {
                Reference = refer
            };

            var execution = _executionService.ExecuteMethod(refer, obj, reference, signature);
            _executionService.SaveExecutionLog(refer.Name, obj.Name, execution);
            return new MethodExecutionResponse() { Execution = execution };
        }
        catch(InvalidOperationLogic ex)
        {
            throw new InvalidExecutionAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InvalidExecutionAdapter ex)
        {
            throw new InvalidExecutionAdapter(ex.Message);
        }
    }

    public bool IsAuthorizedUser(Guid apiKey)
    {
        return _executionService.IsAuthorizedUser(apiKey);
    }
}

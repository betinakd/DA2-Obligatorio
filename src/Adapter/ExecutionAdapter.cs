using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using IAdapter;
using IBussinesLogic;
using Models.Request;

namespace Adapter;

public class ExecutionAdapter(IExecutionService executionService, ISimClassService simClassService) : IExecutionAdapter
{
    private readonly IExecutionService _executionService = executionService;
    private readonly ISimClassService _simClassService = simClassService;

    public string ExecuteMethod(MethodExecutionRequest request)
    {
        try
        {
            var parameters = new List<ParameterSignature>();
            foreach(var parameter in request.Parameters)
            {
                var typeParameter = _simClassService.GetSimClassById(parameter.ClassTypeId);
                var par = new ParameterSignature()
                {
                    Name = parameter.Name,
                    Type = typeParameter,
                };

                parameters.Add(par);
            }

            var signature = new Signature()
            {
                Parameters = parameters,
                Name = request.MethodName
            };
            var refer = _simClassService.GetSimClassById(request.ReferenceTypeId);
            var obj = _simClassService.GetSimClassById(request.InstanceTypeId);
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
    }
}

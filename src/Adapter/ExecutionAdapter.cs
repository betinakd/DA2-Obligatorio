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

            var reference = new ReferenceThis()
            {
                Reference = _simClassService.GetSimClassById(request.ReferenceTypeId)
            };
            var objToCreate = new ReferenceThis()
            {
                Reference = _simClassService.GetSimClassById(request.InstanceTypeId)
            };

            return _executionService.ExecuteMethod(reference, objToCreate, signature);
        }
        catch(InvalidOperationLogic ex)
        {
            throw new InvalidExecutionAdapter(ex.Message);
        }
    }
}

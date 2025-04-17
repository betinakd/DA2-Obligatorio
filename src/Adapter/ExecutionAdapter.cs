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
        var parameters = new List<Parameter>();
        foreach(var parameter in request.Parameters)
        {
            var typeParameter = _simClassService.GetSimClassById(parameter.ClassTypeId);
            var par = new Parameter()
            {
                Name = parameter.Name,
                Type = typeParameter,
            };

            parameters.Add(par);
        }

        return _executionService.ExecuteMethod(request.MethodName, parameters, request.InstanceTypeId, request.ReferenceTypeId, request.InstanceName);
    }
}

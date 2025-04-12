using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

public class SimClassAdapter(ISimClassService simClassService) : ISimClassAdapter
{
    private readonly ISimClassService _simClassService = simClassService;

    public IList<SimClassResponse> GetAllSimClasses()
    {
        var classes = _simClassService.GetAllSimClasses();
        var responses = classes.Select(c => new SimClassResponse(c)).ToList();
        return responses;
    }

    public SimClassResponse CreateSimClass(SimClassRequest request)
    {
        var simClass = _simClassService.CreateSimClass(request.Name, request.IsAbstract, request.IsSealed, request.BaseClassId);
        return new SimClassResponse(simClass);
    }
}
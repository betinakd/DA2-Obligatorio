using Adapter.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;
public class SimClassAdapter(ISimClassService simClassService) : ISimClassAdapter
{
    private readonly ISimClassService _simClassService = simClassService;

    public IList<SimClassResponse> GetAllSimClasses()
    {
        var classes = _simClassService.GetAllSimClasses();
        var responses = classes.Select(c => new SimClassResponse(c)).ToList();
        return responses;
    }

    public CreatedSimClassResponse CreateSimClass(SimClassRequest request)
    {
        var simClass = _simClassService.CreateSimClass(request.Name, request.IsAbstract, request.IsSealed, request.BaseClassId);
        return new CreatedSimClassResponse() { Id = simClass.Id, Message = "Class created successfully", SimClass = new SimClassResponse(simClass) };
    }

    public SimClassResponse UpdateSimClass(UpdateSimClassRequest request)
    {
        try
        {
            var simClass = _simClassService.UpdateSimClass(new SimClass { Id = request.Id, Name = request.Name });
            return new SimClassResponse(simClass);
        }
        catch(SimClassInvalidAttribute ex)
        {
            throw new InvalidAttribute(ex.Message);
        }
    }
}

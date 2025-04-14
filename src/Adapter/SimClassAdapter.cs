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

    public UpdateSimClassResponse UpdateSimClass(UpdateSimClassRequest request)
    {
        try
        {
            var simClass = _simClassService.UpdateSimClass(new SimClass { Id = request.Id, Name = request.Name });
            return new UpdateSimClassResponse() { Message = "Class updated successfully", SimClass = new SimClassResponse(simClass) };
        }
        catch(SimClassInvalidAttribute ex)
        {
            throw new InvalidAttribute(ex.Message);
        }
    }

    public SimClassResponse GetSimClassInfo(Guid classId)
    {
        var simClass = _simClassService.GetSimClassById(classId.ToString());
        if(simClass == null)
        {
            throw new ArgumentException("SimClass not found", nameof(classId));
        }

        var simClassResponse = new SimClassResponse(simClass);
        return simClassResponse;
    }
}

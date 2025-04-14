using Adapter.Exceptions;
using Domain;
using Domain.Exceptions;
using Adapter.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

public class SimClassAdapter(ISimClassService simClassService)
    : ISimClassAdapter
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

    public void DeleteSimClass(Guid id)
    {
        var classes = _simClassService.GetAllSimClasses();
        var classExists = classes.Any(c => c.Id == id);

        if(!classExists)
        {
            throw new ObjectNotFoundException($"Any class with the specified {id} id exists.");
        }
    }
}

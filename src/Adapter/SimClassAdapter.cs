using Adapter.Exceptions;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;
public class SimClassAdapter(ISimClassService simClassService)
    : ISimClassAdapter
{
    private readonly ISimClassService _simClassService = simClassService;

    public IList<SimClassResponse> GetAllSimClasses()
    {
        var classes = _simClassService.GetAllSimClasses();
        var responses = classes.Select(c => new SimClassResponse()
        {
            Id = c.Id,
            Name = c.Name,
            State = EnumMapper.MapToModelAccesibility(c.State),
            Message = "Class retrieved successfully"
        }).ToList();
        return responses;
    }

    public CreatedSimClassResponse CreateSimClass(SimClassRequest request)
    {
        try
        {
            var simClass = _simClassService.CreateSimClass(request.Name, EnumMapper.MapToDomainAccesibility(request.State), request.BaseClassId);
            return new CreatedSimClassResponse() { Id = simClass.Id, Message = "Class created successfully", SimClass = new SimClassResponse() { Id = simClass.Id, Name = request.Name, State = request.State } };
        }
        catch(InUseValueLogic ex)
        {
            throw new InUseException(ex.Message);
        }
    }

    public UpdateSimClassResponse UpdateSimClass(UpdateSimClassRequest request)
    {
        try
        {
            var simClass = _simClassService.UpdateSimClass(new SimClass { Id = request.Id, Name = request.Name, State = EnumMapper.MapToDomainAccesibility(request.State) });
            return new UpdateSimClassResponse() { Message = "Class updated successfully", SimClass = new SimClassResponse() { Id = simClass.Id, Name = simClass.Name, State = request.State } };
        }
        catch(SimClassInvalidAttribute ex)
        {
            throw new InvalidAttribute(ex.Message);
        }
    }

    public void DeleteSimClass(Guid id)
    {
        try
        {
            _simClassService.DeleteSimClass(id);
        }
        catch(Exception)
        {
            throw new ObjectNotFoundException($"Any class with the specified {id} id exists.");
        }
    }

    public SimClassResponse GetSimClassInfo(Guid classId)
    {
        try
        {
            var simClass = _simClassService.GetSimClassById(classId);
            var simClassResponse = new SimClassResponse()
            {
                Id = simClass.Id,
                Message = "Class retrieved successfully",
                Name = simClass.Name,
                State = EnumMapper.MapToModelAccesibility(simClass.State)
            };
            return simClassResponse;
        }
        catch(Exception)
        {
            throw new ObjectNotFoundException("SimClass not found");
        }
    }
}

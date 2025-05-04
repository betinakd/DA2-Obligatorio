using Adapter.Exceptions;
using Adapter.Helpers;
using BussinesLogic.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IBussinesLogic;
using Models.Request;
using Models.Response;

namespace Adapter;
public class SimClassAdapter(ISimClassService simClassService, IExecutionService executionService)
    : ISimClassAdapter
{
    private readonly ISimClassService _simClassService = simClassService;
    private readonly IExecutionService _executionService = executionService;

    public IList<SimClassResponse> GetAllSimClasses()
    {
        return _simClassService.GetAllSimClasses()
            .Select(SimClassResponseMapper.MapToSimClassResponse)
            .ToList();
    }

    public CreatedSimClassResponse CreateSimClass(SimClassRequestCreate request)
    {
        try
        {
            var simClass = _simClassService.CreateSimClass(request.Name, EnumMapper.MapToDomainAccesibility(request.State), request.BaseClassId);
            return new CreatedSimClassResponse() { Message = "Class created successfully", SimClass = new SimClassResponse() { Id = simClass.Id, Name = request.Name, State = (Models.Enums.SimModelsAccesibility)request.State, IdBaseClass = simClass.BaseClassId } };
        }
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(NonExistentValueLogic)
        {
            throw new NonExistentValueAdapter("Base class not found.");
        }
    }

    public UpdateSimClassResponse UpdateSimClass(SimClassRequestUpdate request, Guid idSimClass)
    {
        try
        {
            var baseClass = _simClassService.GetSimClassById(request.BaseClassId);
            var methodsNewClass = new List<SimMethod>();
            var attributesNewClas = new List<SimAttribute>();
            var classToUpdate = new SimClass()
            {
                Id = idSimClass,
                Name = request.Name,
                State = EnumMapper.MapToDomainAccesibility(request.State),
            };

            foreach(var atri in request.Attributes)
            {
                var typeClass = _simClassService.GetSimClassById(atri.ClassTypeId);
                var newAttribute = new SimAttribute()
                {
                    Name = atri.Name,
                    Privacity = EnumMapper.MapToDomainPrivacity(atri.Privacity),
                    RelatedClassId = idSimClass,
                    RelatedClass = classToUpdate,
                    Type = typeClass,
                    TypeId = typeClass.Id
                };
                attributesNewClas.Add(newAttribute);
            }

            foreach(var method in request.Methods)
            {
                var newMethod = new SimMethod()
                {
                    Accesibility = EnumMapper.MapToDomainAccesibility(method.Accesibility),
                    Name = method.Name,
                    ReturnType = _simClassService.GetSimClassById(method.ReturnTypeId),
                    Privacity = EnumMapper.MapToDomainPrivacity(method.Privacity),
                    RelatedClassId = idSimClass,
                    ReturnTypeId = method.ReturnTypeId,
                    RelatedClass = classToUpdate,
                };

                var parametersNewClass = new List<Parameter>();
                var index = 0;
                foreach(var param in method.Parameters)
                {
                    var parameterType = _simClassService.GetSimClassById(param.ClassTypeId);
                    var newParam = new Parameter()
                    {
                        Name = param.Name,
                        Type = parameterType,
                        TypeId = param.ClassTypeId,
                        RelatedMethod = newMethod,
                        RelatedMethodId = newMethod.Id,
                        Index = index
                    };
                    index++;
                    parametersNewClass.Add(newParam);
                }

                _executionService.MethodIsOverridingSealed(idSimClass, newMethod);
                newMethod.Parameters = parametersNewClass;
                methodsNewClass.Add(newMethod);
            }

            classToUpdate.Attributes = attributesNewClas;
            classToUpdate.Methods = methodsNewClass;
            classToUpdate.SetBaseClass(baseClass);
            classToUpdate.BaseClassId = baseClass.Id;
            _simClassService.UpdateSimClass(classToUpdate);

            return new UpdateSimClassResponse() { Message = "Class updated successfully", SimClass = SimClassResponseMapper.MapToSimClassResponse(classToUpdate) };
        }
        catch(InvalidAttributeDomain ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InvalidAttributeLogic ex)
        {
            throw new InvalidAttributeAdapter(ex.Message);
        }
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }

    public void DeleteSimClass(Guid id)
    {
        try
        {
            _simClassService.DeleteSimClass(id);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
        catch(InUseValueLogic ex)
        {
            throw new InUseValueAdapter(ex.Message);
        }
    }

    public SimClassResponse GetSimClassInfo(Guid classId)
    {
        try
        {
            var simClass = _simClassService.GetSimClassById(classId);
            return SimClassResponseMapper.MapToSimClassResponse(simClass);
        }
        catch(NonExistentValueLogic ex)
        {
            throw new NonExistentValueAdapter(ex.Message);
        }
    }
}

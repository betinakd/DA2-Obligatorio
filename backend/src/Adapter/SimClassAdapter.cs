using Adapter.Helpers;
using BusinessLogic.Exceptions;
using Domain;
using Domain.Exceptions;
using IAdapter;
using IAdapter.Exceptions;
using IBusinessLogic;
using Models.Request;
using Models.Response;

namespace Adapter;

public class SimClassAdapter(ISimClassService simClassService, IMethodService methodService)
    : ISimClassAdapter
{
    private readonly ISimClassService _simClassService = simClassService;
    private readonly IMethodService _methodService = methodService;

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
            if(request.BaseClassId == Guid.Empty)
            {
                request.IdBaseClass = "11111111-1111-1111-1111-111111111111";
            }

            var simClass = _simClassService.CreateSimClass(request.Name, EnumMapper.MapToDomainAccesibility(request.State), request.BaseClassId, request.BaseNamespaceId);
            return new CreatedSimClassResponse() { Message = "Class created successfully", SimClass = SimClassResponseMapper.MapToSimClassResponse(simClass) };
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
            if(request.BaseClassId == Guid.Empty)
            {
                request.IdBaseClass = "11111111-1111-1111-1111-111111111111";
            }

            var baseClass = _simClassService.GetSimClassById(request.BaseClassId);
            var methodsNewClass = new List<SimMethod>();
            var attributesNewClas = new List<SimAttribute>();
            var classToUpdate = new SimClass()
            {
                Id = idSimClass,
                Name = request.Name,
                State = EnumMapper.MapToDomainAccesibility(request.State),
                NamespaceId = request.BaseNamespaceId,
            };

            foreach(var atri in request.Attributes)
            {
                var typeClass = _simClassService.GetSimClassById(atri.ReferenceId);
                var instance = _simClassService.GetSimClassById(atri.InstanceId);

                _simClassService.ValidPolymorphism(typeClass, instance);

                var newAttribute = new SimAttribute()
                {
                    Name = atri.Name,
                    Privacity = EnumMapper.MapToDomainPrivacity(atri.Privacity),
                    RelatedClassId = idSimClass,
                    RelatedClass = classToUpdate,
                    Reference = typeClass,
                    ReferenceId = typeClass.Id,
                    Instance = instance,
                    InstanceId = instance.Id,
                    IsStatic = atri.IsStatic
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
                    IsStatic = method.IsStatic,
                    IsVirtual = method.IsVirtual,
                    IsOverride = method.IsOverride,
                };

                var parametersNewClass = new List<Parameter>();
                var index = 0;
                foreach(var param in method.Parameters)
                {
                    var parameterType = _simClassService.GetSimClassById(param.ReferenceId);
                    var newParam = new Parameter()
                    {
                        Name = param.Name,
                        Reference = parameterType,
                        ReferenceId = param.ReferenceId,
                        RelatedMethod = newMethod,
                        RelatedMethodId = newMethod.Id,
                        Index = index
                    };
                    index++;
                    parametersNewClass.Add(newParam);
                }

                newMethod.Parameters = parametersNewClass;
                newMethod.Validate();
                _methodService.IsValidVirtualOverride(request.BaseClassId, newMethod);
                methodsNewClass.Add(newMethod);
            }

            var interfaces = new List<SimClass>();
            foreach(var inter in request.Implements)
            {
                var interfaceClass = _simClassService.GetSimClassById(inter.InterfaceId);
                interfaces.Add(interfaceClass);
            }

            classToUpdate.Attributes = attributesNewClas;
            classToUpdate.Methods = methodsNewClass;
            classToUpdate.SetBaseClass(baseClass);
            classToUpdate.BaseClassId = baseClass.Id;
            classToUpdate.SetImplements(interfaces);
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

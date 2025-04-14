using Domain;

namespace IBussinesLogic;

public interface ISimClassService
{
    IEnumerable<SimClass> GetAllSimClasses();
    SimClass CreateSimClass(string? name, bool? isAbstract, bool? isSealed, Guid? baseClassId);
    SimClass UpdateSimClass(SimClass simClass);
    SimClass GetSimClassById(string id);
}

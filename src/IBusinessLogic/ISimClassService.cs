using Domain;
using Domain.Enums;

namespace IBusinessLogic;

public interface ISimClassService
{
    IList<SimClass> GetAllSimClasses();
    SimClass CreateSimClass(string name, SimAccesibility state, Guid baseClassId, Guid? namespaceId);
    SimClass UpdateSimClass(SimClass simClass);
    SimClass GetSimClassById(Guid id);
    void DeleteSimClass(Guid id);
    SimClass AddInterface(Guid id, Guid interfaceId);
    public void ClassInheritAttribute(Guid idClass, Guid idAttribute);
}

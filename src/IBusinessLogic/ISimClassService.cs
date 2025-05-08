using Domain;
using Domain.Enums;

namespace IBussinesLogic;

public interface ISimClassService
{
    IList<SimClass> GetAllSimClasses();
    SimClass CreateSimClass(string name, SimAccesibility state, Guid baseClassId);
    SimClass UpdateSimClass(SimClass simClass);
    SimClass GetSimClassById(Guid id);
    void DeleteSimClass(Guid id);
}

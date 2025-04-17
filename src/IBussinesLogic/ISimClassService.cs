using Domain;

namespace IBussinesLogic;

public interface ISimClassService
{
    IList<SimClass> GetAllSimClasses();
    SimClass CreateSimClass(string name, bool isAbstract, bool isSealed, Guid baseClassId);
    SimClass UpdateSimClass(SimClass simClass);
    SimClass GetSimClassById(Guid id);
    void DeleteSimClass(Guid id);
}

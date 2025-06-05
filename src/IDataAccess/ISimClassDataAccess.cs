using Domain;

namespace IDataAccess;

public interface ISimClassDataAccess
{
    public SimClass GetSimClassById(Guid id);
    public void CreateSimClass(SimClass simClass);
    public void DeleteSimClass(Guid id);
    public IList<SimClass> GetAllSimClasses();
    public bool ExistSimClassById(Guid id);
    public void UpdateSimClass(SimClass simClass);
    public bool InUseByOther(Guid id);
    public bool ExistSimClassName(string name);
    public bool ClassInheritAttribute(Guid classId, Guid attributeId, int level = 0);
}

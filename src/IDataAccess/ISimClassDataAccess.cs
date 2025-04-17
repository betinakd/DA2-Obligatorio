using Domain;

namespace IDataAccess;

public interface ISimClassDataAccess
{
    public SimClass GetSimClassById(Guid id);
    public void CreateSimClass(SimClass simClass);
    public void DeleteSimClass(Guid id);
    public IList<SimClass> GetAllSimClasses();
    public bool ExistSimClassById(Guid id);
}

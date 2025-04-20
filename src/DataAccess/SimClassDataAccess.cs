using System.Diagnostics.CodeAnalysis;
using Domain;
using IDataAccess;

namespace DataAccess;
[ExcludeFromCodeCoverage]
public class SimClassDataAccess : ISimClassDataAccess
{
    public void CreateSimClass(SimClass simClass)
    {
        throw new NotImplementedException();
    }

    public void DeleteSimClass(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool ExistSimClassById(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool ExistSimClassName(string name)
    {
        throw new NotImplementedException();
    }

    public IList<SimClass> GetAllSimClasses()
    {
        throw new NotImplementedException();
    }

    public SimClass GetSimClassById(Guid id)
    {
        throw new NotImplementedException();
    }

    public bool InUseByOther(Guid id)
    {
        throw new NotImplementedException();
    }

    public void UpdateSimClass(SimClass simClass)
    {
        throw new NotImplementedException();
    }
}

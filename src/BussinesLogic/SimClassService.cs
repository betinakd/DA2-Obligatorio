using BussinesLogic.Exceptions;
using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class SimClassService(ISimClassDataAccess simClassDA) : ISimClassService
{
    private readonly ISimClassDataAccess _simClassDA = simClassDA;
    public SimClass CreateSimClass(string name, bool isAbstract, bool isSealed, Guid baseClassId)
    {
        try
        {
            var baseClass = _simClassDA.GetSimClassById(baseClassId);
            var simClass = new SimClass()
            {
                Id = Guid.NewGuid(),
                Name = name,
                BaseClass = baseClass
            };
            _simClassDA.CreateSimClass(simClass);
            return simClass;
        }
        catch(Exception ex)
        {
            throw new DuplicateValueLogic(ex.Message);
        }
    }

    public void DeleteSimClass(Guid id)
    {
        if(!_simClassDA.ExistSimClassById(id))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        if(_simClassDA.InUseByOther(id))
        {
            throw new InUseValueLogic("SimClass is in use and cannot be updated.");
        }

        _simClassDA.DeleteSimClass(id);
    }

    public IList<SimClass> GetAllSimClasses()
    {
        return _simClassDA.GetAllSimClasses();
    }

    public SimClass GetSimClassById(Guid id)
    {
        if(_simClassDA.ExistSimClassById(id))
        {
            return _simClassDA.GetSimClassById(id);
        }
        else
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }
    }

    public SimClass UpdateSimClass(SimClass simClass)
    {
        if(!_simClassDA.ExistSimClassById(simClass.Id))
        {
            throw new NonExistentValueLogic("SimClass not found.");
        }

        if(_simClassDA.InUseByOther(simClass.Id))
        {
            throw new InUseValueLogic("SimClass is in use and cannot be updated.");
        }

        _simClassDA.UpdateSimClass(simClass);
        return simClass;
    }
}

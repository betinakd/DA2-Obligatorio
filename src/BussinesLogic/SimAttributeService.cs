using BussinesLogic.Exceptions;
using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class SimAttributeService(ISimAttributeDataAccess simAttributeDA, ISimClassDataAccess simClassDA) : ISimAttributeService
{
    private readonly ISimAttributeDataAccess _simAttributeDA = simAttributeDA;
    private readonly ISimClassDataAccess _simClassDA = simClassDA;

    public SimAttribute CreateAttribute(Guid classId, SimAttribute attribute)
    {
        if(!_simClassDA.ExistSimClassById(classId))
        {
            throw new NonExistentValueLogic("Class does not exist.");
        }

        if(_simAttributeDA.ExistAttributeName(classId, attribute.Name))
        {
            throw new InUseValueLogic("Attribute name already exists.");
        }

        return _simAttributeDA.CreateAttribute(classId, attribute);
    }

    public void DeleteAttribute(Guid attributeId)
    {
        if(!_simAttributeDA.ExistAttributeById(attributeId))
        {
            throw new NonExistentValueLogic("Attribute does not exist.");
        }

        if(_simAttributeDA.InUseByOther(attributeId))
        {
            throw new InUseValueLogic("Attribute is in use by another entity.");
        }

        _simAttributeDA.DeleteAttribute(attributeId);
    }

    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute)
    {
        throw new NotImplementedException();
    }
}

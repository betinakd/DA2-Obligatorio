using Domain;
using IBussinesLogic;
using IDataAccess;

namespace BussinesLogic;

public class SimAttributeService(ISimAttributeDataAccess simAttributeDA) : ISimAttributeService
{
    private readonly ISimAttributeDataAccess _simAttributeDA = simAttributeDA;

    public SimAttribute CreateAttribute(Guid claseId, SimAttribute attribute)
    {
        return _simAttributeDA.CreateAttribute(claseId, attribute);
    }

    public void DeleteAttribute(Guid attributeId)
    {
        throw new NotImplementedException();
    }

    public SimAttribute UpdateAttribute(Guid attributeId, SimAttribute attribute)
    {
        throw new NotImplementedException();
    }
}

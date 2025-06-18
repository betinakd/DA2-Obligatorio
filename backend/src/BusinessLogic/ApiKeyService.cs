using IBusinessLogic;
using IDataAccess;

namespace BusinessLogic;

public class ApikeyService(IApikeyDataAccess apikeyDataAccess) : IApikeyService
{
    private readonly IApikeyDataAccess _apikeyDA = apikeyDataAccess;

    public bool IsAuthorizedUser(Guid apiKey)
    {
        var keyExists = _apikeyDA.ApiKeyExists(apiKey);
        return keyExists && !(apiKey == Guid.Empty);
    }
}

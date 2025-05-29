namespace IDataAccess;

public interface IApikeyDataAccess
{
    bool ApiKeyExists(Guid keyValue);
}

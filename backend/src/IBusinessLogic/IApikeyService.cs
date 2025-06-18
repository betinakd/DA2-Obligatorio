namespace IBusinessLogic;

public interface IApikeyService
{
    public bool IsAuthorizedUser(Guid apiKey);
}

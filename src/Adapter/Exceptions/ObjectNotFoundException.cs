namespace Adapter.Exceptions;

public class ObjectNotFoundException(string message)
    : Exception(message)
{
}

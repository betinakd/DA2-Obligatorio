namespace Adapter.Exceptions;

public class NonExistentValueAdapter(string message)
    : Exception(message)
{
}

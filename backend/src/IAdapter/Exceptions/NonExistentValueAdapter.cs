namespace IAdapter.Exceptions;

public class NonExistentValueAdapter(string message)
    : Exception(message)
{
}

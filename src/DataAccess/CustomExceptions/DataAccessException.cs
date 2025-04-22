namespace DataAccess.CustomExceptions;

public class DataAccessException(string message, Exception? exceptionMessage) : Exception(message, exceptionMessage)
{
}

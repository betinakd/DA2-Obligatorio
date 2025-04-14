namespace Domain.Validations;

public class SyntaxisValidation
{
    public static bool IsValidName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        char[] invalidSymbols = ['"', '@', '#', '$', '%', '&', '*', '!', '?', '/', '\\', '=', '+', '(', ')', '{', '}'];
        return !name.Any(c => invalidSymbols.Contains(c));
    }
}
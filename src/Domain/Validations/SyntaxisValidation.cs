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

    public static bool OnlyNumbers(string input)
    {
        return input.All(char.IsDigit);
    }

    public static bool ReservedWords(string word)
    {
        string[] reservedWords = ["class", "public", "private", "protected", "internal", "static", "var"];
        return reservedWords.Contains(word.ToLower());
    }
}

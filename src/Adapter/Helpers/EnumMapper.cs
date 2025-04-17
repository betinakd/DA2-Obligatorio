using Adapter.Exceptions;
using Domain.Enums;

public static class EnumMapper
{
    public static SimPrivacity GetPrivacity(string value)
    {
        return Enum.TryParse(value, true, out SimPrivacity privacity) ? privacity : throw new InvalidAttribute("Invalid privacity value.");
    }

    public static SimAccesibility GetState(string value)
    {
        return Enum.TryParse(value, true, out SimAccesibility state) ? state : throw new InvalidAttribute("Invalid state value.");
    }
}

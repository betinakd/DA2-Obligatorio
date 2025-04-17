using Domain.Enums;
using Models.Enums;

public static class EnumMapper
{
    public static SimModelsPrivacity MapToModelPrivacity(SimPrivacity privacity)
    {
        return (SimModelsPrivacity)(int)privacity;
    }

    public static SimModelsAccesibility MapToModelAccesibility(SimAccesibility accesibility)
    {
        return (SimModelsAccesibility)(int)accesibility;
    }
}

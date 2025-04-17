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

    public static SimPrivacity MapToDomainPrivacity(SimModelsPrivacity privacity)
    {
        return (SimPrivacity)(int)privacity;
    }

    public static SimAccesibility MapToDomainAccesibility(SimModelsAccesibility accesibility)
    {
        return (SimAccesibility)(int)accesibility;
    }
}

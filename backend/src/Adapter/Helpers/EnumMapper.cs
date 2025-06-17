using Domain.Enums;
using Models.Enums;

namespace Adapter.Helpers;

public class EnumMapper
{
    public static SimModelsPrivacity MapToModelPrivacity(SimPrivacity? privacity)
    {
        return privacity.HasValue ? (SimModelsPrivacity)(int)privacity.Value : default;
    }

    public static SimModelsAccesibility MapToModelAccesibility(SimAccesibility? accesibility)
    {
        return accesibility.HasValue ? (SimModelsAccesibility)(int)accesibility.Value : default;
    }

    public static SimPrivacity MapToDomainPrivacity(SimModelsPrivacity? privacity)
    {
        return (SimPrivacity)(int)privacity;
    }

    public static SimAccesibility MapToDomainAccesibility(SimModelsAccesibility? accesibility)
    {
        return (SimAccesibility)(int)accesibility;
    }
}

using Domain;
using Domain.Enums;
namespace Models.Response;

public class SimClassResponse(SimClass simClass)
{
    public Guid? Id { get; set; } = simClass.Id;
public string? Name { get; set; } = simClass.Name;
public bool? IsAbstract { get; set; } = simClass.GetState() == SimState.Abstract;
public bool? IsSealed { get; set; } = simClass.GetState() == SimState.Sealed;
}

using Domain.Enums;

namespace Domain;

public class SimClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public Guid? BaseClassId { get; set; }
    public StateClass State { get; set; } = new StateNormal();

    public SimState GetState()
    {
        return State.GetState();
    }
}

using Domain.Enums;

namespace Domain;

public class SimClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public Guid? BaseClassId { get; set; }
    private StateClass _state = new StateNormal();

    public SimState GetState()
    {
        return _state.GetState();
    }

    public void SetState(StateClass state)
    {
        _state = state;
    }
}

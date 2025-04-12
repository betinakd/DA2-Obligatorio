using Domain.Enums;

namespace Domain;

public class StateNormal : StateClass
{
    public override SimState GetState()
    {
        return SimState.Normal;
    }
}

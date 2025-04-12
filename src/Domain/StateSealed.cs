using Domain.Enums;

namespace Domain;

    public class StateSealed : StateClass
{
    public override SimState GetState()
    {
        return SimState.Sealed;
    }
}

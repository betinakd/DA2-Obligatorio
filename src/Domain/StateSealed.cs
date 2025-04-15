using Domain.Enums;

namespace Domain;

public class StateSealed : StateClass
{
    public override SimAccesibility GetState()
    {
        return SimAccesibility.Sealed;
    }
}

using Domain.Enums;

namespace Domain;

public class StateNormal : StateClass
{
    public override SimAccesibility GetState()
    {
        return SimAccesibility.Normal;
    }
}

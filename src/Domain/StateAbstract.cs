using Domain.Enums;

namespace Domain;

public class StateAbstract : StateClass
{
    public override SimState GetState()
    {
        return SimState.Abstract;
    }
}

using Domain.Enums;

namespace Domain;

public class StateAbstract : StateClass
{
    public override SimAccesibility GetState()
    {
        return SimAccesibility.Abstract;
    }
}

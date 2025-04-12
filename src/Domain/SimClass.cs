using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class SimClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    private SimClass? _baseClassField = null;
    private StateClass? _state = new StateNormal();

    public SimState GetState()
    {
        return _state?.GetState() ?? throw new InvalidOperationException("State is not set.");
    }

    public void SetState(StateClass state)
    {
        _state = state;
    }

    public SimClass? BaseClass
    {
        get => _baseClassField;
        set
        {
            if(value == null || value?.GetState() == SimState.Sealed)
            {
                throw new SimClassInvalidAttribute("Cannot set as base a sealed or null Class.");
            }

            _baseClassField = value;
        }
    }
}
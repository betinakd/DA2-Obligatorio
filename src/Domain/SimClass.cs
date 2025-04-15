using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class SimClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string? _name;

    private SimClass? _baseClassField = null;
    private StateClass? _state = new StateNormal();

    public List<SimAttribute> Attributes { get; set; } = [];

    public string? Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be null or empty.");
            }

            _name = value;
        }
    }

    public SimAccesibility GetState()
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
            if(value == null || value?.GetState() == SimAccesibility.Sealed)
            {
                throw new SimClassInvalidAttribute("Cannot set as base a sealed or null Class.");
            }

            _baseClassField = value;
        }
    }

    public void AddAttribute(SimAttribute attribute)
    {
        if(Attributes.Any(a => a.Name == attribute.Name))
        {
            throw new SimClassInvalidAttribute("This Class already has an attribute with the same name.");
        }

        Attributes.Add(attribute);
    }

    public void DeleteAttribute(SimAttribute attribute)
    {
        var numberDeleted = Attributes.RemoveAll(a => a.Name == attribute.Name);
        if(numberDeleted == 0)
        {
            throw new SimClassInvalidOperation("No attribute was deleted, it may not exist.");
        }
    }
}

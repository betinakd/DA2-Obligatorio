using Domain.Enums;
using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class SimClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = string.Empty;
    public Guid? BaseClassId { get; set; }

    private SimClass? _baseClassField = null;
    public SimAccesibility State { get; set; } = SimAccesibility.Normal;

    public List<SimAttribute> Attributes { get; set; } = [];
    public List<SimMethod> Methods { get; set; } = [];

    public string Name
    {
        get => _name;
        set
        {
            if(!SyntaxisValidation.IsValidName(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be empty or contain invalid characters.");
            }

            _name = value;
        }
    }

    public SimClass? BaseClass
    {
        get => _baseClassField;
        set
        {
            if(value == null || value?.State == SimAccesibility.Sealed)
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

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
    private List<SimMethod> _methods = [];

    public List<SimMethod> Methods
    {
        get => _methods;

        set
        {
            _methods = value ?? [];

            if(_methods.Any(m => m.Accesibility == SimAccesibility.Abstract))
            {
                State = SimAccesibility.Abstract;
            }
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if(!SyntaxisValidation.IsValidName(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be empty or contain invalid characters.");
            }

            if(SyntaxisValidation.OnlyNumbers(value))
            {
                throw new SimClassInvalidAttribute("Name cannot contain only numbers.");
            }

            if(SyntaxisValidation.ReservedWords(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be a reserved word.");
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

            if(value?.State == SimAccesibility.Abstract && State != SimAccesibility.Abstract)
            {
                var abstractMethods = value.Methods.Where(m => m.Accesibility == SimAccesibility.Abstract).ToList();
                var missingMethods = abstractMethods.Where(am => !Methods.Any(m => m.Name == am.Name)).ToList();

                if(missingMethods.Any())
                {
                    throw new SimClassInvalidAttribute($"The following abstract methods are not implemented: {string.Join(", ", missingMethods.Select(m => m.Name))}");
                }
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

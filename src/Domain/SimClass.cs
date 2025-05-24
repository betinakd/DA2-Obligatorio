using Domain.Enums;
using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class SimClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
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

            if(_methods.Any(m => m.Accesibility != SimAccesibility.Interface && State == SimAccesibility.Interface))
            {
                throw new InvalidAttributeDomain("An interface cannot have non-interface accesibility methods.");
            }
        }
    }

    private Guid? _baseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public Guid? BaseClassId
    {
        get => _baseClassId;
        set
        {
            if(State == SimAccesibility.Abstract && value != Guid.Parse("11111111-1111-1111-1111-111111111111"))
            {
                throw new InvalidAttributeDomain("BaseClassId must be object State is Abstract.");
            }

            if(value == null)
            {
                _baseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            }
        }
    }

    private string _name = string.Empty;
    private SimClass? _baseClassField = null;

    public List<SimAttribute> Attributes { get; set; } = [];

    public string Name
    {
        get => _name;
        set
        {
            if(!SyntaxisValidation.IsValidName(value))
            {
                throw new InvalidAttributeDomain("Name cannot be empty or contain invalid characters.");
            }

            if(SyntaxisValidation.OnlyNumbers(value))
            {
                throw new InvalidAttributeDomain("Name cannot contain only numbers.");
            }

            if(SyntaxisValidation.ReservedWords(value))
            {
                throw new InvalidAttributeDomain("Name cannot be a reserved word.");
            }

            _name = value;
        }
    }

    public SimClass? BaseClass
    {
        get => _baseClassField;
        set
        {
            if(value?.State == SimAccesibility.Sealed)
            {
                throw new InvalidAttributeDomain("Cannot set as base a sealed or null Class.");
            }

            _baseClassField = value;
        }
    }

    private SimAccesibility _state = SimAccesibility.Normal;
    public SimAccesibility State
    {
        get => _state;
        set
        {
            if(value == SimAccesibility.Abstract && _baseClassId != Guid.Parse("11111111-1111-1111-1111-111111111111"))
            {
                throw new InvalidAttributeDomain("BaseClassId must be object when State is Abstract.");
            }

            _state = value;
        }
    }

    public void SetBaseClass(SimClass? value)
    {
        if(value?.State == SimAccesibility.Sealed)
        {
            throw new InvalidAttributeDomain("Cannot set as base a sealed or null Class.");
        }

        if(value?.State == SimAccesibility.Abstract && State != SimAccesibility.Abstract)
        {
            var abstractMethods = value.Methods.Where(m => m.Accesibility == SimAccesibility.Abstract).ToList();
            var missingMethods = abstractMethods.Where(am => !Methods.Any(m => m.Name == am.Name)).ToList();

            if(missingMethods.Any())
            {
                throw new InvalidAttributeDomain($"The following abstract methods are not implemented: {string.Join(", ", missingMethods.Select(m => m.Name))}");
            }
        }

        _baseClassField = value;
    }
}

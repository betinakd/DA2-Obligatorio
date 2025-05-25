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

            if(_methods.Any(m => m.Accesibility == SimAccesibility.Interface && State != SimAccesibility.Interface))
            {
                throw new InvalidAttributeDomain("A non-interface class cannot have interface methods.");
            }

            for(var i = 0; i < _methods.Count; i++)
            {
                for(var j = 0; j < _methods.Count; j++)
                {
                    if(_methods[i].Equals(_methods[j]) && i != j)
                    {
                        throw new InvalidAttributeDomain($"Duplicate method name found: {_methods[i].Name}");
                    }
                }
            }
        }
    }

    private Guid? _baseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public Guid? BaseClassId
    {
        get => _baseClassId;
        set
        {
            if((State == SimAccesibility.Abstract || State == SimAccesibility.Interface) && value != Guid.Parse("11111111-1111-1111-1111-111111111111"))
            {
                throw new InvalidAttributeDomain("Cannot set a base class for an abstract or interface class.");
            }

            if(value == null || value == Guid.Empty)
            {
                _baseClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            }
            else
            {
                _baseClassId = value;
            }
        }
    }

    private string _name = string.Empty;
    private SimClass? _baseClassField = null;
    private List<SimAttribute> _attributes = [];

    public List<SimAttribute> Attributes
    {
        get => _attributes;
        set
        {
            if(State == SimAccesibility.Interface && value.Any())
            {
                throw new InvalidAttributeDomain("An interface cannot have attributes.");
            }

            for(var i = 0; i < value.Count; i++)
            {
                for(var j = 0; j < value.Count; j++)
                {
                    if(value[i].Name.Equals(value[j].Name) && i != j)
                    {
                        throw new InvalidAttributeDomain($"Duplicate attribute name found: {value[i].Name}");
                    }
                }
            }

            _attributes = value;
        }
    }

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
            if(value?.State == SimAccesibility.Sealed || value?.State == SimAccesibility.Interface)
            {
                throw new InvalidAttributeDomain("Cannot set as base a sealed or Interface Class.");
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

            if(value == SimAccesibility.Interface && _baseClassId != Guid.Parse("11111111-1111-1111-1111-111111111111"))
            {
                throw new InvalidAttributeDomain("BaseClassId must be object when State is Interface.");
            }

            _state = value;
        }
    }

    private List<SimClass> _implements = [];
    public List<SimClass> Implements
    {
        get => _implements;
        set
        {
            if(State == SimAccesibility.Interface && value.Any())
            {
                throw new InvalidAttributeDomain("An interface cannot implement other classes.");
            }

            _implements = value;
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

using Domain.Enums;
using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class SimMethod
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = string.Empty;
    public Guid? ReturnTypeId { get; set; }
    public SimClass? ReturnType { get; set; } = null!;
    public Guid RelatedClassId { get; set; }

    public SimClass RelatedClass { get; set; } = null!;
    public SimPrivacity Privacity { get; set; }
    private SimAccesibility _accesibility;

    public SimAccesibility Accesibility
    {
        get => _accesibility;
        set
        {
            if(IsStatic && value != SimAccesibility.Normal)
            {
                throw new InvalidAttributeDomain("Static methods cannot be Abstract, Interface, Sealed accessibility.");
            }

            _accesibility = value;
        }
    }

    private bool _isStatic = false;
    public bool IsStatic
    {
        get => _isStatic;
        set
        {
            if(value && Accesibility != SimAccesibility.Normal)
            {
                throw new InvalidAttributeDomain("Static methods cannot be Abstract, Interface or Sealed accessibility.");
            }

            _isStatic = value;
        }
    }

    private List<Parameter> _parameters = [];
    public List<Parameter> Parameters
    {
        get => _parameters;
        set
        {
            _parameters = value;
        }
    }

    private List<LocalVariable> _localVariables = [];
    public List<LocalVariable> LocalVariables
    {
        get => _localVariables;
        set
        {
            if(Accesibility == SimAccesibility.Interface && value.Any())
            {
                throw new InvalidAttributeDomain("Interface methods cannot have local variables.");
            }

            _localVariables = value;
        }
    }

    private List<Invocation> _invocations = [];
    public List<Invocation> Invocations
    {
        get => _invocations;
        set
        {
            if(Accesibility == SimAccesibility.Interface && value.Any())
            {
                throw new InvalidAttributeDomain("Interface methods cannot have invocations.");
            }

            if(IsStatic && value.Any(i => i.Reference.GetReferenceTypeDescription() != "Static" && i.Reference.GetReferenceTypeDescription() != "StaticAttribute"))
            {
                throw new InvalidAttributeDomain("Static methods cannot have non-static invocations.");
            }

            _invocations = value;
        }
    }

    public bool MatchSignature(Signature signature)
    {
        if(Name.ToLower() != signature.Name.ToLower() || Parameters.Count != signature.Parameters.Count)
        {
            return false;
        }

        for(var i = 0; i < Parameters.Count; i++)
        {
            if(Parameters[i].TypeId != signature.Parameters[i].TypeId)
            {
                return false;
            }
        }

        return true;
    }

    public string GetMethodSignature(Signature signature)
    {
        var simParams = string.Join(", ", signature.Parameters.Select(p => p.Name));
        return RelatedClass.Name + "." + signature.Name + "(" + simParams + ")";
    }

    public override bool Equals(object? obj)
    {
        return EqualsWithReturnType(obj);
    }

    public bool EqualsWithoutReturnType(object? obj)
    {
        if(obj is not SimMethod otherMethod)
        {
            return false;
        }

        if(Name.ToLower() != otherMethod.Name.ToLower() || Parameters.Count != otherMethod.Parameters.Count)
        {
            return false;
        }

        for(var i = 0; i < Parameters.Count; i++)
        {
            if(Parameters[i].TypeId != otherMethod.Parameters[i].TypeId)
            {
                return false;
            }
        }

        return true;
    }

    private bool EqualsWithReturnType(object? otherMethod)
    {
        if(otherMethod is not SimMethod obj)
        {
            return false;
        }

        return EqualsWithoutReturnType(obj) && (ReturnTypeId == obj.ReturnTypeId);
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
                throw new InvalidAttributeDomain("Name cannot be only numbers.");
            }

            if(SyntaxisValidation.ReservedWords(value))
            {
                throw new InvalidAttributeDomain("Name cannot be a reserved word.");
            }

            _name = value;
        }
    }

    public override string ToString()
    {
        return Privacity + " " + Accesibility + " " + ReturnType.Name + " " + Name + "(" + string.Join(", ", Parameters.Select(p => p.Name)) + ")";
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

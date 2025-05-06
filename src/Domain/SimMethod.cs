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
    public SimAccesibility Accesibility { get; set; }
    public List<Parameter> Parameters { get; set; } = [];
    public List<LocalVariable> LocalVariables { get; set; } = [];
    public List<Invocation> Invocations { get; set; } = [];

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
        if(obj is not SimMethod otherMethod)
        {
            return false;
        }

        if(Name.ToLower() != otherMethod.Name.ToLower() || Parameters.Count != otherMethod.Parameters.Count)
        {
            return false;
        }

        foreach(var parameter in Parameters)
        {
            var parameterMatched = false;
            foreach(var otherParam in otherMethod.Parameters)
            {
                var typeIdsMatch = parameter.TypeId == otherParam.TypeId;
                if(typeIdsMatch)
                {
                    parameterMatched = true;
                    break;
                }
            }

            if(!parameterMatched)
            {
                return false;
            }
        }

        return true;
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

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

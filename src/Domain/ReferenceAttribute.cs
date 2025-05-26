using Domain.Exceptions;

namespace Domain;

public class ReferenceAttribute : Reference
{
    private SimAttribute _reference = new SimAttribute();
    public SimAttribute Reference
    {
        get => _reference;
        set
        {
            if(value != null && value.IsStatic)
            {
                throw new InvalidAttributeDomain("Static attributes are not allowed. Use ReferenceStaticAttribute instead.");
            }

            _reference = value;
        }
    }

    public Guid ReferenceId { get; set; }

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return Reference.Name + "." + method.Name + "(" + simParams + ")";
    }

    public override string GetSignatureWithClassName(Signature signature)
    {
        var simParams = string.Join(", ", signature.Parameters.Select(p => p.Name));
        return Reference.Type.Name + "." + signature.Name + "(" + simParams + ")";
    }

    public override SimClass GetSimClass()
    {
        if(Reference.Type == null)
        {
            throw new InvalidAttributeDomain("Reference type cannot be null.");
        }

        return Reference.Type;
    }

    public override Guid GetReferenceId()
    {
        return Reference.Id;
    }

    public override string GetReferenceTypeDescription()
    {
        return "Attribute";
    }
}

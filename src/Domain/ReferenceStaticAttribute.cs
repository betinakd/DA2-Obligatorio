using Domain.Exceptions;

namespace Domain;

public class ReferenceStaticAttribute : Reference
{
    private SimAttribute _reference = new SimAttribute();
    public SimAttribute Reference
    {
        get => _reference;
        set
        {
            if(value != null && !value.IsStatic)
            {
                throw new InvalidAttributeDomain("Only static attributes allowed on ReferenceStaticAttribute.");
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
        return Reference.Reference.Name + "." + signature.Name + "(" + simParams + ")";
    }

    public override SimClass GetReferenceClass()
    {
        if(Reference.Reference == null)
        {
            throw new InvalidAttributeDomain("Reference type cannot be null.");
        }

        return Reference.Reference;
    }

    public override Guid GetReferenceId()
    {
        return Reference.Id;
    }

    public override string GetReferenceTypeDescription()
    {
        return "StaticAttribute";
    }

    public override SimClass GetInstanceClass(Signature signature, SimClass executionInstance)
    {
        return Reference.Instance;
    }
}

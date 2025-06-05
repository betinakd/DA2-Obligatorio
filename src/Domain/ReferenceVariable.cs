using Domain.Exceptions;

namespace Domain;

public class ReferenceVariable : Reference
{
    public LocalVariable Reference { get; set; } = new LocalVariable();
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
        return "LocalVariable";
    }
}

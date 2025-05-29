using Domain.Exceptions;

namespace Domain;

public class ReferenceBase : Reference
{
    public SimClass Reference { get; set; } = new SimClass();
    public Guid ReferenceId { get; set; }

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return "base" + "." + method.Name + "(" + simParams + ")";
    }

    public override string GetSignatureWithClassName(Signature signature)
    {
        var simParams = string.Join(", ", signature.Parameters.Select(p => p.Name));
        return Reference.BaseClass.Name + "." + signature.Name + "(" + simParams + ")";
    }

    public override SimClass GetReferenceClass()
    {
        if(Reference.BaseClass == null)
        {
            throw new InvalidAttributeDomain("Base Class is null.");
        }

        return Reference.BaseClass;
    }

    public override Guid GetReferenceId()
    {
        return Reference.Id;
    }

    public override string GetReferenceTypeDescription()
    {
        return "Base";
    }
}

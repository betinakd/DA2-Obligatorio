using Domain;
using Domain.Exceptions;

public class ReferenceParameter : Reference
{
    public Parameter Reference { get; set; } = new Parameter();
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
}

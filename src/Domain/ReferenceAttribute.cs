using Domain;
using Domain.Exceptions;

public class ReferenceAttribute : Reference
{
    public SimAttribute Reference { get; set; } = new SimAttribute();
    public Guid ReferenceId { get; set; }

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return Reference.Name + "." + method.Name + "(" + simParams + ")";
    }

    public override SimClass GetSimClass()
    {
        if(Reference.Type == null)
        {
            throw new SimClassInvalidAttribute("Reference type cannot be null.");
        }

        return Reference.Type;
    }
}

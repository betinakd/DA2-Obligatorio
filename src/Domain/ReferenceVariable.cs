using Domain;
using Domain.Exceptions;

public class ReferenceVariable : Reference
{
    public LocalVariable Reference { get; set; } = new LocalVariable();

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return "this" + "." + method.Name + "(" + simParams + ")";
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

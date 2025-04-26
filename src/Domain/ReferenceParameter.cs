using Domain;
using Domain.Exceptions;

public class ReferenceParameter : Reference
{
    public Parameter Reference { get; set; } = new Parameter();

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

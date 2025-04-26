using Domain;
using Domain.Exceptions;

public class ReferenceBase : Reference
{
    public SimClass Reference { get; set; } = new SimClass();

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return "this" + "." + method.Name + "(" + simParams + ")";
    }

    public override SimClass GetSimClass()
    {
        if(Reference.BaseClass == null)
        {
            throw new SimClassInvalidAttribute("Base Class is null.");
        }

        return Reference.BaseClass;
    }
}

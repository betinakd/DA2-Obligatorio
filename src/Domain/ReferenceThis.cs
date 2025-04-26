using Domain;

public class ReferenceThis : Reference
{
    public SimClass Reference { get; set; } = new SimClass();

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return "this" + "." + method.Name + "(" + simParams + ")";
    }

    public override SimClass GetSimClass()
    {
        return Reference;
    }
}

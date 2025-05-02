namespace Domain;

public class ReferenceThis : Reference
{
    public SimClass Reference { get; set; } = new SimClass();
    public Guid ReferenceId { get; set; }

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ", method.Parameters.Select(p => p.Name));
        return "this" + "." + method.Name + "(" + simParams + ")";
    }

    public override string GetSignatureWithClassName(Signature signature)
    {
        var simParams = string.Join(", ", signature.Parameters.Select(p => p.Name));
        return Reference.Name + "." + signature.Name + "(" + simParams + ")";
    }

    public override SimClass GetSimClass()
    {
        return Reference;
    }

    public override Guid GetReferenceId()
    {
        return Reference.Id;
    }

    public override string GetReferenceTypeDescription()
    {
        return "This";
    }
}

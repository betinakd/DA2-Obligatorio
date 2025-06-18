namespace Domain;

public class ReferenceStatic : Reference
{
    public SimClass Reference { get; set; } = new SimClass();
    public Guid ReferenceId { get; set; }

    public override string GetSignature(Signature method)
    {
        var simParams = string.Join(", ",
            method.Parameters.Select(p =>
                $"{p.Name}: {p.Reference?.Name ?? "null"} {p.Instance?.Name ?? "null"}"));
        return Reference.Name + "." + method.Name + "(" + simParams + ")";
    }

    public override string GetSignatureWithClassName(Signature signature)
    {
        var simParams = string.Join(", ",
            signature.Parameters.Select(p =>
                $"{p.Name}: {p.Reference?.Name ?? "null"} {p.Instance?.Name ?? "null"}"));
        return Reference.Name + "." + signature.Name + "(" + simParams + ")";
    }

    public override SimClass GetReferenceClass()
    {
        return Reference;
    }

    public override Guid GetReferenceId()
    {
        return Reference.Id;
    }

    public override string GetReferenceTypeDescription()
    {
        return "Static";
    }

    public override SimClass GetInstanceClass(Signature signature, SimClass executionInstance)
    {
        return Reference;
    }

    public override bool UsesDynamicDispatch()
    {
        return false;
    }
}

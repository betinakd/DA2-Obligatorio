using Domain.Exceptions;

namespace Domain;

public class ReferenceParameter : Reference
{
    public Parameter Reference { get; set; } = new Parameter();
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
        return "Parameter";
    }

    public override SimClass GetInstanceClass(Signature signature, SimClass executionInstance)
    {
        if(signature.Parameters != null)
        {
            var param = signature.Parameters.FirstOrDefault(p => p.Index == Reference.Index);
            if(param != null)
            {
                return param.Instance;
            }
        }

        return Reference.Reference;
    }

    public override bool UsesDynamicDispatch()
    {
        return true;
    }
}

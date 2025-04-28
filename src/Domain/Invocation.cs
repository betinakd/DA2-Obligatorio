using Domain.Exceptions;

namespace Domain;
public class Invocation()
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private Reference? _reference;
    public Reference Reference
    {
        get => _reference;
        set
        {
            if(value == null)
            {
                throw new SimClassInvalidAttribute("Reference cannot be null.");
            }

            _reference = value;
        }
    }

    public Guid? ReferenceId { get; set; }

    private Signature _signature = new Signature();
    public Signature Signature
    {
        get => _signature;
        set
        {
            if(value == null)
            {
                throw new SimClassInvalidAttribute("Signature cannot be null.");
            }

            if(string.IsNullOrWhiteSpace(value.Name))
            {
                throw new SimClassInvalidAttribute("Method name in signature cannot be empty.");
            }

            _signature = value;
        }
    }

    public Guid? SignatureId { get; set; }

    private Guid? _relatedMethodId;
    public Guid? RelatedMethodId
    {
        get => _relatedMethodId;
        set
        {
            if(value == null)
            {
                throw new SimClassInvalidAttribute("Related method ID cannot be null or empty.");
            }

            _relatedMethodId = null;
        }
    }

    public SimMethod? RelatedMethod { get; set; }
}

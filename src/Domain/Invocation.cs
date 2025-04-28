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

            _signature = null;
        }
    }

    public Guid? SignatureId { get; set; }

    public Guid? RelatedMethodId { get; set; }
    public SimMethod? RelatedMethod { get; set; }
}

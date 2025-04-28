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

            _reference = null;
        }
    }

    public Guid? ReferenceId { get; set; }

    public Signature Signature { get; set; } = new Signature();

    public Guid? SignatureId { get; set; }

    public Guid? RelatedMethodId { get; set; }
    public SimMethod? RelatedMethod { get; set; }
}

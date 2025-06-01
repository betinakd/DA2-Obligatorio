using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class LocalVariable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = string.Empty;
    public Guid ReferenceId { get; set; } = Guid.Empty;
    public SimClass Reference { get; set; } = null!;
    public Guid InstanceId { get; set; } = Guid.Empty;
    public SimClass Instance { get; set; } = null!;
    public Guid? RelatedMethodId { get; set; }
    public SimMethod? RelatedMethod { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            if(!SyntaxisValidation.IsValidName(value))
            {
                throw new InvalidAttributeDomain("Name cannot be empty or contain invalid characters.");
            }

            if(SyntaxisValidation.OnlyNumbers(value))
            {
                throw new InvalidAttributeDomain("Name cannot be only numbers.");
            }

            if(SyntaxisValidation.ReservedWords(value))
            {
                throw new InvalidAttributeDomain("Name cannot be a reserved word.");
            }

            _name = value;
        }
    }
}

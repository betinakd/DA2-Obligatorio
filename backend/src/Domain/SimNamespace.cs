using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class SimNamespace
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public SimNamespace? BaseNamespace { get; set; }
    public Guid? BaseNamespaceId { get; set; } = null;
    public List<SimClass> Elements { get; set; } = [];

    private string _name = string.Empty;
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
                throw new InvalidAttributeDomain("Name cannot contain only numbers.");
            }

            if(SyntaxisValidation.ReservedWords(value))
            {
                throw new InvalidAttributeDomain("Name cannot be a reserved word.");
            }

            _name = value;
        }
    }
}

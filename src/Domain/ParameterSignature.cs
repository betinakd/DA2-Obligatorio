using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class ParameterSignature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = string.Empty;
    public SimClass? Reference { get; set; }
    public Guid ReferenceId { get; set; }
    public Guid InstanceId { get; set; }
    public SimClass? Instance { get; set; }
    public Guid? SignatureId { get; set; }
    public Signature? Signature { get; set; }
    public int Index { get; set; }

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

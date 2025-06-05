using Domain.Enums;
using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class SimAttribute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = string.Empty;
    public Guid? ReferenceId { get; set; }
    public SimClass? Reference { get; set; }
    public SimPrivacity? Privacity { get; set; } = SimPrivacity.Public;
    public bool IsStatic { get; set; } = false;
    public SimClass? RelatedClass { get; set; }
    public Guid? RelatedClassId { get; set; }
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

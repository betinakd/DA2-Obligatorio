using Domain.Enums;
using Domain.Exceptions;
using Domain.Validations;

namespace Domain;

public class SimAttribute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    private string _name = string.Empty;
    public SimClass? Type { get; set; }
    public SimAccesibility Accesibility { get; set; } = SimAccesibility.Public;
    public required SimClass RelatedClass { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            if(!SyntaxisValidation.IsValidName(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be empty or contain invalid characters.");
            }

            if(SyntaxisValidation.OnlyNumbers(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be only numbers.");
            }

            if(SyntaxisValidation.ReservedWords(value))
            {
                throw new SimClassInvalidAttribute("Name cannot be a reserved word.");
            }

            _name = value;
        }
    }
}

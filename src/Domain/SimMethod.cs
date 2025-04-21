using Domain.Enums;

namespace Domain;

public class SimMethod
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid? ReturTypeId { get; set; }
    public SimClass? ReturnType { get; set; } = null!;
    public Guid? RelatedClassId { get; set; }

    public SimClass RelatedClass { get; set; } = null!;
    public SimPrivacity Privacity { get; set; }
    public SimAccesibility Accesibility { get; set; }
    public List<Parameter> Parameters { get; set; } = [];
    public List<LocalVariable> LocalVariables { get; set; } = [];
    public List<Invocation> Invocations { get; set; } = [];
}

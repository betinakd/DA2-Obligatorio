using Domain;

namespace IBussinesLogic;

public interface IMethodService
{
    IEnumerable<SimMethod> GetAllSimMethods();
    SimMethod CreateClassMethod(string? name, string? returnType, string? accessModifier, bool? isStatic, bool? isAbstract, Guid? classId);
    SimMethod AddMethodParameter(Guid id, string? name, string? type);
}

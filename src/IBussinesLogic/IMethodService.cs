using Domain;

namespace IBussinesLogic;

public interface IMethodService
{
    IEnumerable<SimMethod> GetAllSimMethods();
    SimMethod CreateClassMethod(string? name, string? returnType, string? accessModifier, bool? isStatic, bool? isAbstract, Guid? classId);
    SimAttribute AddMethodParameter(Guid methodId, string? name, string? type);
    SimAttribute AddLocalVariable(Guid methodId, string? name, string? type);
}

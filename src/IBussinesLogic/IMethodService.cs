using Domain;

namespace IBussinesLogic;

public interface IMethodService
{
    SimMethod GetMethodById(Guid id);
    SimMethod CreateClassMethod(string? name, string? returnType, string? accessModifier, bool? isStatic, bool? isAbstract, Guid? classId);
    SimAttribute AddMethodParameter(Guid methodId, Parameter parameter);
    SimAttribute AddLocalVariable(Guid methodId, LocalVariable localVariable);
    Parameter GetParameterById(Guid id);
    LocalVariable GetVariableById(Guid id);
    SimMethod AddMethod(Guid id, SimMethod method);
}

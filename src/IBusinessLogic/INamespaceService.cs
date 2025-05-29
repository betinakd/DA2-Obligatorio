using Domain;
using Models.Request;

namespace IBusinessLogic;

public interface INamespaceService
{
    public SimNamespace CreateNamespace(NamespaceRequest simNamespace);
}

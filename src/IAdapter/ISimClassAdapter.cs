using Models.Request;
using Models.Response;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    CreatedSimClassResponse CreateSimClass(SimClassRequestCreate request);
    UpdateSimClassResponse UpdateSimClass(SimClassRequestUpdate request, Guid idSimClass);
    void DeleteSimClass(Guid id);
    SimClassResponse GetSimClassInfo(Guid classId);
    SimClassResponse AddInterface(Guid id, InterfaceRequestUpdate methodRequest);
}

using Models.Request;
using Models.Response;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    CreatedSimClassResponse CreateSimClass(SimClassRequestUpdate request);
    UpdateSimClassResponse UpdateSimClass(SimClassRequestCreate request, Guid idSimClass);
    void DeleteSimClass(Guid id);
    SimClassResponse GetSimClassInfo(Guid classId);
}

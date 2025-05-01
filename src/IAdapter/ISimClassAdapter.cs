using Models.Request;
using Models.Response;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    CreatedSimClassResponse CreateSimClass(SimClassRequest request);
    UpdateSimClassResponse UpdateSimClass(SimClassRequestCreateClass request, Guid idSimClass);
    void DeleteSimClass(Guid id);
    SimClassResponse GetSimClassInfo(Guid classId);
}

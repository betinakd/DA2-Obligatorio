using Models.Request;
using Models.Response;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    CreatedSimClassResponse CreateSimClass(SimClassRequest request);
    SimClassResponse UpdateSimClass(UpdateSimClassRequest request);
}

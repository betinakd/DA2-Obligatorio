using Models.Request;
using Models.Response;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    SimClassResponse CreateSimClass(SimClassRequest request);
}

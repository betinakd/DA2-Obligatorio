using Models.Response;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
}
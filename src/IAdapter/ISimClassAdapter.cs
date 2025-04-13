using Models.Request;
using Models.Response;
using System;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    SimClassResponse CreateSimClass(SimClassRequest request);
    void DeleteSimClass(Guid id);
}

using Models.Request;
using Models.Response;
using System;

namespace IAdapter;

public interface ISimClassAdapter
{
    IList<SimClassResponse> GetAllSimClasses();
    CreatedSimClassResponse CreateSimClass(SimClassRequest request);
    UpdateSimClassResponse UpdateSimClass(UpdateSimClassRequest request);
    void DeleteSimClass(Guid id);
}

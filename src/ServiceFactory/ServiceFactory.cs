using BussinesLogic;
using DataAccess.Context;
using IAdapter;
using IBussinesLogic;
using IDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceFactory;

public static class ServiceFactory
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISimClassAdapter, SimClassAdapter>();
        services.AddScoped<ISimClassService, SimClassService>();
        services.AddScoped<ISimClassDataAccess, SimClassDataAccess>();
        services.AddScoped<IMethodAdapter, MethodAdapter>();
        services.AddScoped<IMethodService, MethodService>();
        services.AddScoped<ISimMethodDataAccess, MethodDataAccess>();
        services.AddScoped<IAttributeAdapter, AttributeAdapter>();
        services.AddScoped<ISimAttributeService, SimAttributeService>();
        services.AddScoped<ISimAttributeDataAccess, SimAttributeDataAccess>();
        services.AddScoped<IExecutionAdapter, ExecutionAdapter>();
        services.AddScoped<IExecutionService, ExecutionService>();
        services.AddScoped<IExecutionDataAccess, ExecutionDataAccess>();
        services.AddDbContext<SimulatorDbContext>(options =>
            options.UseSqlServer("DefaultConnection"));

        return services;
    }
}

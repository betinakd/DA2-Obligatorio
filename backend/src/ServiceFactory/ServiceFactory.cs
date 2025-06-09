using System.Diagnostics.CodeAnalysis;
using Adapter;
using BusinessLogic;
using DataAccess;
using DataAccess.Context;
using IAdapter;
using IBusinessLogic;
using IDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceFactory;

[ExcludeFromCodeCoverage]
public static class SimulatorServiceFactory
{
    public static IServiceCollection AddServices(this IServiceCollection services, string? connectionString)
    {
        services.AddScoped<INamespaceAdapter, NamespaceAdapter>();
        services.AddScoped<INamespaceService, NamespaceService>();
        services.AddScoped<INamespaceDataAccess, NamespaceDataAccess>();
        services.AddScoped<ISimClassAdapter, SimClassAdapter>();
        services.AddScoped<ISimClassService, SimClassService>();
        services.AddScoped<ISimClassDataAccess, SimClassDataAccess>();
        services.AddScoped<IMethodAdapter, MethodAdapter>();
        services.AddScoped<IMethodService, SimMethodService>();
        services.AddScoped<ISimMethodDataAccess, SimMethodDataAccess>();
        services.AddScoped<IAttributeAdapter, AttributeAdapter>();
        services.AddScoped<ISimAttributeService, SimAttributeService>();
        services.AddScoped<ISimAttributeDataAccess, SimAttributeDataAccess>();
        services.AddScoped<IExecutionAdapter, ExecutionAdapter>();
        services.AddScoped<IExecutionService, ExecutionService>();
        services.AddScoped<IApikeyService, ApikeyService>();
        services.AddScoped<IApikeyDataAccess, ApikeyDataAccess>();
        services.AddScoped<IExecutionDataAccess, ExecutionDataAccess>();
        services.AddScoped<ITransformerAdapter, TransformerAdapter>();
        services.AddScoped<ITransformerService, TransformerService>();
        services.AddDbContext<DbContext, SimulatorDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(30000);
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));
        return services;
    }
}

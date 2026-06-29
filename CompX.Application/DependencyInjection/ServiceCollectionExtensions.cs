using CompX.Application.Abstractions.Services;
using CompX.Application.Services;
using Microsoft.Extensions.DependencyInjection;

using SchedulingHistoryService = CompX.Application.Services.SchedulingHistory;

namespace CompX.Application.DependencyInjection;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IHealthApplicationService, HealthApplicationService>();
        services.AddScoped<ISampleApplicationService, SampleApplicationService>();
        services.AddScoped<IAccountReceivableApplicationService, AccountReceivableApplicationService>();
        services.AddScoped<ISchedulingApplicationService, SchedulingApplicationService>();
        services.AddScoped<ISchedulingHistory, SchedulingHistoryService>();
        services.AddScoped<IBillingApplicationService, BillingApplicationService>();
        services.AddScoped<IManageApplicationService, ManageApplicationService>();
        services.AddScoped<IClaimantService, ClaimantService>();
        return services;
    }
}

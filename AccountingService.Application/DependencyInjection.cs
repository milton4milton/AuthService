using AccountingService.Application.Interfaces;
using AccountingService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFinancialYearService, FinancialYearService>();
        services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        return services;
    }
}

using Ikhtibar.API.Middleware;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services;
using Ikhtibar.Infrastructure.Repositories;
using Ikhtibar.Infrastructure.Services;

namespace Ikhtibar.API.Extensions;

/// <summary>
/// Extension methods for configuring audit logging services
/// </summary>
public static class AuditServiceExtensions
{
    /// <summary>
    /// Adds audit logging services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for method chaining</returns>
    public static IServiceCollection AddAuditServices(this IServiceCollection services)
    {
        // Register HTTP context accessor (required by AuditService)
        services.AddHttpContextAccessor();
        
        // Register audit repository
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        
        // Register audit service
        services.AddScoped<IAuditService, AuditService>();
        
        return services;
    }
    
    /// <summary>
    /// Adds audit logging middleware to the application pipeline
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder for method chaining</returns>
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<AuditMiddleware>();
        
        return app;
    }
}

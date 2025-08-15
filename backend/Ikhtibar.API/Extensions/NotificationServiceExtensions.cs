// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.Extensions.DependencyInjection;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Infrastructure.Repositories;
using Ikhtibar.Infrastructure.Services;
using Ikhtibar.Infrastructure.Messaging;

namespace Ikhtibar.API.Extensions;

/// <summary>
/// Extension methods for registering notification services
/// </summary>
public static class NotificationServiceExtensions
{
    /// <summary>
    /// Add notification services to the service collection
    /// </summary>
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
        services.AddScoped<INotificationHistoryRepository, NotificationHistoryRepository>();

        // Register services
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
        services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();

        // Register messaging services
        services.AddSingleton<IEventBusService, EventBusService>();
        services.AddSingleton<INotificationQueue, NotificationQueue>();
        services.AddSingleton<NotificationScheduler>();

        return services;
    }
}
*/

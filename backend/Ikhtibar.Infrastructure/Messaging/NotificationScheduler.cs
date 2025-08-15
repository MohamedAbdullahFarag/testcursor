// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Shared.Enums;

namespace Ikhtibar.Infrastructure.Messaging;

/// <summary>
/// Background service for processing scheduled notifications
/// </summary>
public class NotificationScheduler : BackgroundService
{
    private readonly ILogger<NotificationScheduler> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public NotificationScheduler(
        ILogger<NotificationScheduler> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification scheduler started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessScheduledNotificationsAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in notification scheduler");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait longer on error
            }
        }

        _logger.LogInformation("Notification scheduler stopped");
    }

    private async Task ProcessScheduledNotificationsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var result = await notificationService.ProcessScheduledNotificationsAsync();
            if (result.Success && result.Data > 0)
            {
                _logger.LogInformation("Processed {Count} scheduled notifications", result.Data);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing scheduled notifications");
        }
    }
}
*/

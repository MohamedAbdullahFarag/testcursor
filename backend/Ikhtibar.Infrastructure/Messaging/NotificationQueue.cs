// Temporarily commented out to resolve build errors
// Notification system is being reworked and will be re-enabled later
/*
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Ikhtibar.Infrastructure.Messaging;

/// <summary>
/// Simple in-memory notification queue service
/// </summary>
public interface INotificationQueue
{
    Task EnqueueAsync(NotificationQueueItem item);
    Task<NotificationQueueItem?> DequeueAsync();
    Task<int> GetQueueLengthAsync();
    Task ClearQueueAsync();
}

/// <summary>
/// Notification queue item
/// </summary>
public class NotificationQueueItem
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public object Data { get; set; } = null!;
    public DateTime EnqueuedAt { get; set; }
    public int Priority { get; set; }
    public int RetryCount { get; set; }
}

/// <summary>
/// In-memory notification queue implementation
/// </summary>
public class NotificationQueue : INotificationQueue
{
    private readonly ILogger<NotificationQueue> _logger;
    private readonly ConcurrentQueue<NotificationQueueItem> _queue;
    private readonly object _lockObject = new object();

    public NotificationQueue(ILogger<NotificationQueue> logger)
    {
        _logger = logger;
        _queue = new ConcurrentQueue<NotificationQueueItem>();
    }

    public async Task EnqueueAsync(NotificationQueueItem item)
    {
        try
        {
            item.EnqueuedAt = DateTime.UtcNow;
            _queue.Enqueue(item);
            
            _logger.LogDebug("Notification {Type} enqueued with priority {Priority}", item.Type, item.Priority);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enqueueing notification {Type}", item.Type);
            throw;
        }
    }

    public async Task<NotificationQueueItem?> DequeueAsync()
    {
        try
        {
            if (_queue.TryDequeue(out var item))
            {
                _logger.LogDebug("Notification {Type} dequeued", item.Type);
                return item;
            }
            
            await Task.CompletedTask;
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dequeuing notification");
            throw;
        }
    }

    public async Task<int> GetQueueLengthAsync()
    {
        await Task.CompletedTask;
        return _queue.Count;
    }

    public async Task ClearQueueAsync()
    {
        try
        {
            lock (_lockObject)
            {
                while (_queue.TryDequeue(out _)) { }
            }
            
            _logger.LogInformation("Notification queue cleared");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing notification queue");
            throw;
        }
    }
}
*/

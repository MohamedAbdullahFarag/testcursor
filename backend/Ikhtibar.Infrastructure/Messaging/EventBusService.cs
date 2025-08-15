using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Messaging;

/// <summary>
/// Simple event bus service for managing event publishing and subscription
/// In a production environment, consider using a more robust solution like MediatR or MassTransit
/// </summary>
public interface IEventBusService
{
    Task PublishAsync<T>(T @event) where T : class;
    void Subscribe<T>(Func<T, Task> handler) where T : class;
}

public class EventBusService : IEventBusService
{
    private readonly ILogger<EventBusService> _logger;
    private readonly Dictionary<Type, List<Func<object, Task>>> _handlers = new();

    public EventBusService(ILogger<EventBusService> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        try
        {
            _logger.LogInformation("Publishing event of type {EventType}", typeof(T).Name);

            if (_handlers.TryGetValue(typeof(T), out var handlers))
            {
                var tasks = handlers.Select(handler => handler(@event));
                await Task.WhenAll(tasks);
                
                _logger.LogInformation("Successfully published event of type {EventType} to {HandlerCount} handlers", 
                    typeof(T).Name, handlers.Count);
            }
            else
            {
                _logger.LogDebug("No handlers registered for event type {EventType}", typeof(T).Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event of type {EventType}", typeof(T).Name);
            throw;
        }
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        var eventType = typeof(T);
        
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Func<object, Task>>();
        }

        _handlers[eventType].Add(async obj => await handler((T)obj));
        
        _logger.LogInformation("Registered handler for event type {EventType}", eventType.Name);
    }
}

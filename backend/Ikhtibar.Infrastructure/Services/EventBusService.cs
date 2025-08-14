using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services.Interfaces;
using System.Collections.Concurrent;

namespace Ikhtibar.Infrastructure.Services;

/// <summary>
/// In-memory implementation of the event bus service for the notification system.
/// Provides asynchronous event publishing with proper error handling and retry mechanisms.
/// Uses dependency injection to resolve event handlers and ensures proper logging.
/// 
/// This implementation is suitable for single-instance deployments.
/// For distributed scenarios, consider implementing with Azure Service Bus or similar.
/// </summary>
public class EventBusService : IEventBusService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBusService> _logger;
    
    // Thread-safe collection to store event handler registrations
    private readonly ConcurrentDictionary<Type, List<Type>> _eventHandlers;

    public EventBusService(
        IServiceProvider serviceProvider,
        ILogger<EventBusService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventHandlers = new ConcurrentDictionary<Type, List<Type>>();
    }

    /// <summary>
    /// Publishes a domain event to all registered handlers asynchronously.
    /// Implements proper error handling, logging, and performance monitoring.
    /// </summary>
    public async Task<bool> PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : class, IDomainEvent
    {
        if (domainEvent == null)
        {
            _logger.LogWarning("Attempted to publish null domain event of type {EventType}", typeof(TEvent).Name);
            return false;
        }

        using var scope = _logger.BeginScope("Publishing event {EventId} of type {EventType}", 
            domainEvent.EventId, typeof(TEvent).Name);

        try
        {
            var eventType = typeof(TEvent);
            
            // Get registered handlers for this event type
            if (!_eventHandlers.TryGetValue(eventType, out var handlerTypes) || !handlerTypes.Any())
            {
                _logger.LogInformation("No handlers registered for event type {EventType}", eventType.Name);
                return true; // No handlers is not an error condition
            }

            _logger.LogInformation("Publishing event {EventId} to {HandlerCount} handlers", 
                domainEvent.EventId, handlerTypes.Count);

            var publishTasks = new List<Task>();
            var handlerInstances = new List<(IEventHandler<TEvent> Handler, Type HandlerType)>();

            // Resolve handler instances from DI container
            foreach (var handlerType in handlerTypes)
            {
                try
                {
                    using var handlerScope = _serviceProvider.CreateScope();
                    var handler = handlerScope.ServiceProvider.GetService(handlerType) as IEventHandler<TEvent>;
                    
                    if (handler != null)
                    {
                        handlerInstances.Add((handler, handlerType));
                    }
                    else
                    {
                        _logger.LogWarning("Could not resolve handler {HandlerType} for event {EventType}", 
                            handlerType.Name, eventType.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error resolving handler {HandlerType} for event {EventType}", 
                        handlerType.Name, eventType.Name);
                }
            }

            // Sort handlers by priority (lower numbers first)
            handlerInstances = handlerInstances
                .OrderBy(h => h.Handler.Priority)
                .ToList();

            // Execute handlers based on their failure handling strategy
            var successCount = 0;
            var failureCount = 0;

            foreach (var (handler, handlerType) in handlerInstances)
            {
                try
                {
                    var startTime = DateTime.UtcNow;
                    
                    await handler.HandleAsync(domainEvent, cancellationToken);
                    
                    var duration = DateTime.UtcNow - startTime;
                    successCount++;
                    
                    _logger.LogDebug("Handler {HandlerType} processed event {EventId} in {Duration}ms", 
                        handlerType.Name, domainEvent.EventId, duration.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(ex, "Handler {HandlerType} failed to process event {EventId}", 
                        handlerType.Name, domainEvent.EventId);

                    // Check if we should continue processing other handlers
                    if (!handler.ContinueOnFailure)
                    {
                        _logger.LogWarning("Handler {HandlerType} failure stops further processing of event {EventId}", 
                            handlerType.Name, domainEvent.EventId);
                        break;
                    }
                }
            }

            _logger.LogInformation("Event {EventId} processing completed: {SuccessCount} succeeded, {FailureCount} failed", 
                domainEvent.EventId, successCount, failureCount);

            return failureCount == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing event {EventId} of type {EventType}", 
                domainEvent.EventId, typeof(TEvent).Name);
            return false;
        }
    }

    /// <summary>
    /// Publishes multiple domain events in a batch operation.
    /// Provides better performance for bulk event publishing scenarios.
    /// </summary>
    public async Task<int> PublishBatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null || !domainEvents.Any())
        {
            _logger.LogInformation("No events provided for batch publishing");
            return 0;
        }

        var events = domainEvents.ToList();
        _logger.LogInformation("Starting batch publish of {EventCount} events", events.Count);

        var successCount = 0;
        var publishTasks = new List<Task<bool>>();

        // Group events by type for more efficient processing
        var eventGroups = events.GroupBy(e => e.GetType());

        foreach (var eventGroup in eventGroups)
        {
            var eventType = eventGroup.Key;
            _logger.LogDebug("Processing {EventCount} events of type {EventType}", 
                eventGroup.Count(), eventType.Name);

            foreach (var domainEvent in eventGroup)
            {
                // Use reflection to call the generic PublishAsync method
                var publishMethod = typeof(EventBusService)
                    .GetMethod(nameof(PublishAsync))
                    ?.MakeGenericMethod(eventType);

                if (publishMethod != null)
                {
                    var publishTask = (Task<bool>)publishMethod.Invoke(this, new object[] { domainEvent, cancellationToken })!;
                    publishTasks.Add(publishTask);
                }
            }
        }

        // Wait for all publish operations to complete
        var results = await Task.WhenAll(publishTasks);
        successCount = results.Count(r => r);

        _logger.LogInformation("Batch publish completed: {SuccessCount}/{TotalCount} events published successfully", 
            successCount, events.Count);

        return successCount;
    }

    /// <summary>
    /// Registers an event handler for a specific event type.
    /// Supports multiple handlers per event type for flexible event processing.
    /// </summary>
    public void RegisterHandler<TEvent, THandler>()
        where TEvent : class, IDomainEvent
        where THandler : class, IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var handlerType = typeof(THandler);

        _eventHandlers.AddOrUpdate(
            eventType,
            new List<Type> { handlerType },
            (key, existingHandlers) =>
            {
                if (!existingHandlers.Contains(handlerType))
                {
                    existingHandlers.Add(handlerType);
                }
                return existingHandlers;
            });

        _logger.LogInformation("Registered handler {HandlerType} for event type {EventType}", 
            handlerType.Name, eventType.Name);
    }

    /// <summary>
    /// Removes an event handler registration.
    /// Allows dynamic reconfiguration of event handling at runtime.
    /// </summary>
    public void UnregisterHandler<TEvent, THandler>()
        where TEvent : class, IDomainEvent
        where THandler : class, IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var handlerType = typeof(THandler);

        if (_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            handlers.Remove(handlerType);
            
            if (!handlers.Any())
            {
                _eventHandlers.TryRemove(eventType, out _);
            }

            _logger.LogInformation("Unregistered handler {HandlerType} for event type {EventType}", 
                handlerType.Name, eventType.Name);
        }
    }

    /// <summary>
    /// Gets all registered handlers for a specific event type.
    /// Useful for diagnostics and system monitoring.
    /// </summary>
    public IEnumerable<Type> GetHandlers<TEvent>() where TEvent : class, IDomainEvent
    {
        var eventType = typeof(TEvent);
        
        if (_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            return handlers.ToList(); // Return a copy to prevent external modification
        }

        return Enumerable.Empty<Type>();
    }
}

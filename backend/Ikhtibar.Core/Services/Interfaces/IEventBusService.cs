using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Core.Services.Interfaces;

/// <summary>
/// Event bus service for publishing and handling domain events within the notification system.
/// Provides asynchronous event publishing with proper error handling and retry mechanisms.
/// Integrates with the notification system to trigger automatic notifications based on domain events.
/// </summary>
public interface IEventBusService
{
    /// <summary>
    /// Publishes a domain event to all registered handlers asynchronously.
    /// Ensures proper error handling and logging for failed event processing.
    /// </summary>
    /// <typeparam name="TEvent">Type of domain event to publish</typeparam>
    /// <param name="domainEvent">The event instance to publish</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if event was published successfully, false otherwise</returns>
    Task<bool> PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : class, IDomainEvent;

    /// <summary>
    /// Publishes multiple domain events in a batch operation.
    /// Provides better performance for bulk event publishing scenarios.
    /// </summary>
    /// <param name="domainEvents">Collection of events to publish</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of events successfully published</returns>
    Task<int> PublishBatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers an event handler for a specific event type.
    /// Supports multiple handlers per event type for flexible event processing.
    /// </summary>
    /// <typeparam name="TEvent">Type of event to handle</typeparam>
    /// <typeparam name="THandler">Type of handler to register</typeparam>
    void RegisterHandler<TEvent, THandler>()
        where TEvent : class, IDomainEvent
        where THandler : class, IEventHandler<TEvent>;

    /// <summary>
    /// Removes an event handler registration.
    /// Allows dynamic reconfiguration of event handling at runtime.
    /// </summary>
    /// <typeparam name="TEvent">Type of event</typeparam>
    /// <typeparam name="THandler">Type of handler to unregister</typeparam>
    void UnregisterHandler<TEvent, THandler>()
        where TEvent : class, IDomainEvent
        where THandler : class, IEventHandler<TEvent>;

    /// <summary>
    /// Gets all registered handlers for a specific event type.
    /// Useful for diagnostics and system monitoring.
    /// </summary>
    /// <typeparam name="TEvent">Type of event</typeparam>
    /// <returns>Collection of registered handler types</returns>
    IEnumerable<Type> GetHandlers<TEvent>() where TEvent : class, IDomainEvent;
}

/// <summary>
/// Base interface for all domain events in the notification system.
/// Provides common properties and metadata for event processing.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event instance.
    /// Used for tracking and deduplication purposes.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Timestamp when the event was created.
    /// Used for event ordering and audit trails.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Identifier of the user who triggered the event.
    /// Used for authorization and audit logging.
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// Optional correlation identifier for related events.
    /// Helps track event chains and business processes.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Event metadata for additional context.
    /// Allows flexible event enrichment without breaking changes.
    /// </summary>
    Dictionary<string, object> Metadata { get; }
}

/// <summary>
/// Interface for event handlers that process specific domain events.
/// Implements the handler pattern for clean event processing separation.
/// </summary>
/// <typeparam name="TEvent">Type of event this handler processes</typeparam>
public interface IEventHandler<in TEvent> where TEvent : class, IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event asynchronously.
    /// Should include proper error handling and logging.
    /// </summary>
    /// <param name="domainEvent">The event to handle</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Task representing the async operation</returns>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the priority order for this handler when multiple handlers exist.
    /// Lower numbers execute first. Default is 0.
    /// </summary>
    int Priority => 0;

    /// <summary>
    /// Indicates whether this handler should continue processing if it fails.
    /// True = continue with other handlers, False = stop processing on failure.
    /// </summary>
    bool ContinueOnFailure => true;
}

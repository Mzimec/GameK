using System;
using R3;
using System.Collections.Generic;

/// <summary>
/// Marker interface representing a generic event channel.
/// </summary>
public interface IEventChannel { }

/// <summary>
/// Represents a typed event channel for events of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The event type handled by this channel.</typeparam>
public interface IEventChannel<T> : IEventChannel 
    where T : IEvent {

    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    /// <param name="evt">The event instance to publish.</param>
    public void Publish(T evt);

    /// <summary>
    /// Returns an observable sequence for this event type.
    /// Subscribers can listen to this to receive events.
    /// </summary>
    /// <returns>An observable of type T.</returns>
    public Observable<T> On();
}

/// <summary>
/// Implementation of a typed event channel using reactive Subject.
/// </summary>
/// <typeparam name="T">The type of event.</typeparam>
public class EventChannel<T> : IEventChannel<T>, IDisposable
    where T : IEvent {

    /// <summary>
    /// Underlying subject that manages subscriptions.
    /// </summary>
    public readonly Subject<T> subject = new Subject<T>();

    /// <inheritdoc/>
    public void Publish(T evt) => subject.OnNext(evt);

    /// <inheritdoc/>
    public Observable<T> On() => subject.AsObservable();

    /// <summary>
    /// Disposes the subject and releases resources.
    /// </summary>
    public void Dispose() {
        subject.Dispose();
    }
}

/// <summary>
/// Represents a generic event bus for publishing and subscribing to events.
/// </summary>
public interface IEventBus {
    /// <summary>
    /// Subscribes to events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of event.</typeparam>
    /// <param name="onEvent">Callback invoked when an event is published.</param>
    /// <returns>IDisposable that can be used to unsubscribe.</returns>
    public IDisposable Subscribe<T>(Action<T> onEvent) where T : IEvent;

    /// <summary>
    /// Publishes an event to the bus.
    /// </summary>
    /// <typeparam name="T">The type of event.</typeparam>
    /// <param name="evt">Event instance.</param>
    public void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// Returns an observable sequence for the specified event type.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <returns>An Observable of type T.</returns>
    public Observable<T> On<T>() where T : IEvent;

    /// <summary>
    /// Clears all event channels and disposes resources.
    /// </summary>
    public void Clear();
}

/// <summary>
/// Base implementation of a generic event bus using typed event channels.
/// </summary>
public class BaseEventBus : IEventBus {
    private readonly Dictionary<Type, IEventChannel> _channels = new Dictionary<Type, IEventChannel>();

    /// <summary>
    /// Gets an existing event channel or creates a new one for type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Event type.</typeparam>
    /// <returns>Event channel for type T.</returns>
    protected EventChannel<T> GetOrCreateChannel<T>() where T : IEvent {
        var type = typeof(T);
        if (!_channels.TryGetValue(type, out var existing)) {
            var newChannel = new EventChannel<T>();
            _channels[type] = newChannel;
            return newChannel;
        }

        return (EventChannel<T>)existing;
    }

    /// <inheritdoc/>
    public IDisposable Subscribe<T>(Action<T> onEvent) where T : IEvent
        => GetOrCreateChannel<T>().On().Subscribe(onEvent);

    /// <inheritdoc/>
    public void Publish<T>(T evt) where T : IEvent
        => GetOrCreateChannel<T>().Publish(evt);

    /// <inheritdoc/>
    public Observable<T> On<T>() where T : IEvent
        => GetOrCreateChannel<T>().On();

    /// <inheritdoc/>
    public void Clear() {
        foreach (var channel in _channels.Values) {
            if (channel is IDisposable d) {
                d.Dispose();
            }
        }
        _channels.Clear();
    }
}

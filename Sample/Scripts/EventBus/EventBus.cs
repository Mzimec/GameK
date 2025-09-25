
using System;
using System.Collections.Generic;
using UnityEngine;

/*public interface IUntypedEventBus {
    public void Clear();
}

public interface IEventBus<T> : IUntypedEventBus
    where T : IEvent {
    public void Register(EventBinding<T> binding);
    public void Unregister(EventBinding<T> binding);
    public void Raise(T evt);
}*/

/// <summary>
/// Represents a global event bus for a specific event type <typeparamref name="T"/>.
/// Provides static registration, unregistration, and raising of events.
/// </summary>
/// <typeparam name="T">Type of the event, must implement IEvent.</typeparam>
public static class GlobalEventBus<T>
    where T : IEvent {

    /// <summary>
    /// The set of registered event bindings for this event type.
    /// </summary>
    static readonly HashSet<IEventBinding<T>> _bindings = new HashSet<IEventBinding<T>>();

    /// <summary>
    /// Registers a binding to this global event bus.
    /// </summary>
    /// <param name="binding">The event binding to register.</param>
    public static void Register(EventBinding<T> binding) => _bindings.Add(binding);

    /// <summary>
    /// Unregisters a binding from this global event bus.
    /// </summary>
    /// <param name="binding">The event binding to remove.</param>
    public static void Unsregister(EventBinding<T> binding) => _bindings.Remove(binding);


    /// <summary>
    /// Raises an event, invoking all registered bindings.
    /// Uses a snapshot to prevent issues if bindings modify the set during invocation.
    /// </summary>
    /// <param name="evt">The event instance to raise.</param>
    public static void Raise(T evt) {
        var snapshot = new HashSet<IEventBinding<T>>(_bindings);

        foreach (var binding in snapshot) {
            if (_bindings.Contains(binding)) {
                binding.OnEvent.Invoke(evt);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }

    /// <summary>
    /// Clears all bindings from this global bus.
    /// </summary>
    static void Clear() {
        Debug.Log($"Clearing {typeof(T).Name} bindings");
        _bindings.Clear();
    }
}

/*public class LocalEventBus<T> : IEventBus<T>
    where T : IEvent {
    private readonly HashSet<IEventBinding<T>> _bindings;

    public LocalEventBus(HashSet<IEventBinding<T>> bindings) => _bindings = bindings;
    public LocalEventBus(LocalEventBus<T> eventBus) => _bindings = new HashSet<IEventBinding<T>>();
    public void Raise(T evt) {
        var snapshot = new HashSet<IEventBinding<T>>(_bindings);

        foreach (var binding in snapshot) {
            if (_bindings.Contains(binding)) {
                binding.OnEvent.Invoke(evt);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }

    public void Register(EventBinding<T> binding) => _bindings.Add(binding);

    public void Unregister(EventBinding<T> binding) => _bindings.Remove(binding);

    public void Clear() => _bindings.Clear();
}

public interface IEventBusContainer<TEvent, TBus>
    where TEvent : IEvent
    where TBus : IEventBus<TEvent> { }

public class EventBusContainer<TEvent, TBus> : IEventBusContainer<TEvent, TBus>
    where TEvent : IEvent
    where TBus : IEventBus<TEvent> {
    private Dictionary<Type, TBus> _eventBuses = new Dictionary<Type, TBus>();

    public EventBusContainer(IList<Type> busTypes, IList<Type> eventTypes) {
        for (int i = 0; i < busTypes.Count; i++) {
            var busType = busTypes[i];
            var eventType = eventTypes[i];
            var bus = (TBus)Activator.CreateInstance(busType);
            _eventBuses.Add(eventType, bus);
        }
    }
    /*public void Register(TBus bus) {
        var eventType = typeof(TEvent);
        if (!_eventBuses.ContainsKey(eventType)) {
            _eventBuses.Add(eventType, bus);
        }
    }
    public void Unregister(TBus bus) {
        var eventType = typeof(TEvent);
        if (_eventBuses.ContainsKey(eventType)) {
            _eventBuses.Remove(eventType);
        }
    }
}

public class CharacterEventBus : IEventBusContainer<ICharacterEvent, IEventBus<ICharacterEvent>> {

    private Dictionary<Type, LocalEventBus<IEvent>> _eventBuses;

    public CharacterEventBus() {
        _eventBuses = new();
        foreach (var (key, value) in EventBusUtil.CharacterEventBuses) {
            _eventBuses[key] = new LocalEventBus<typof(key)>(new HashSet<IEventBinding<IEvent>>());
        }
    }
}*/




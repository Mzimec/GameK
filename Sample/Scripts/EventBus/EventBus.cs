
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

public static class GlobalEventBus<T>
    where T : IEvent {
    static readonly HashSet<IEventBinding<T>> _bindings = new HashSet<IEventBinding<T>>(); 

    public static void Register(EventBinding<T> binding) => _bindings.Add(binding);
    public static void Unsregister(EventBinding<T> binding) => _bindings.Remove(binding);

    public static void Raise(T evt) {
        var snapshot = new HashSet<IEventBinding<T>>(_bindings);

        foreach (var binding in snapshot) {
            if (_bindings.Contains(binding)) {
                binding.OnEvent.Invoke(evt);
                binding.OnEventNoArgs.Invoke();
            }
        }
    }

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




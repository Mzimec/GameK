using System;
using R3;
using System.Collections.Generic;

public interface IEventChannel { }

public interface IEventChannel<T> : IEventChannel 
    where T : IEvent {
    public void Publish(T evt);
    public Observable<T> On();
}

public class EventChannel<T> : IEventChannel<T>, IDisposable
    where T : IEvent {
    public readonly Subject<T> subject = new Subject<T>();
    public void Publish(T evt) => subject.OnNext(evt);
    public Observable<T> On() => subject.AsObservable();
    public void Dispose() {
        subject.Dispose();
    }
}

public interface IEventBus { 
    public IDisposable Subscribe<T>(Action<T> onEvent) where T : IEvent;
    public void Publish<T>(T evt) where T : IEvent;
    public Observable<T> On<T>() where T : IEvent;
    public void Clear();
}

public class BaseEventBus : IEventBus {
    private readonly Dictionary<Type, IEventChannel> _channels = new Dictionary<Type, IEventChannel>();

    protected EventChannel<T> GetOrCreateChannel<T>() where T : IEvent {
        var type = typeof(T);
        if (!_channels.TryGetValue(type, out var existing)) {
            var newChannel = new EventChannel<T>();
            _channels[type] = newChannel;
            return newChannel;
        }

        return (EventChannel<T>)existing;
    }
    public IDisposable Subscribe<T>(Action<T> onEvent) where T : IEvent
        => GetOrCreateChannel<T>().On().Subscribe(onEvent);


    public void Publish<T>(T evt) where T : IEvent
        => GetOrCreateChannel<T>().Publish(evt);

    public Observable<T> On<T>() where T : IEvent
        => GetOrCreateChannel<T>().On();

    public void Clear() {
        foreach (var channel in _channels.Values) {
            if (channel is IDisposable d) {
                d.Dispose();
            }
        }
        _channels.Clear();
    }
}

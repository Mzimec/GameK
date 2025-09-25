using System;


/// <summary>
/// Interface for binding events to actions.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEventBinding<T> {
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
}

/// <summary>
/// Concrete implementation of IEventBinding for a specific event type.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventBinding<T> : IEventBinding<T> where T : IEvent {
    Action<T> onEvent = _ => { };
    Action onEventNoArgs = () => { };

    /// <summary>
    /// Gets or sets the action to be invoked when the event of type T is triggered.
    /// </summary>
    Action<T> IEventBinding<T>.OnEvent {
        get => onEvent;
        set => onEvent = value;
    }

    /// <summary>
    /// Gets or sets the action to be invoked when the event of type T is triggered, without event arguments.
    /// </summary>
    Action IEventBinding<T>.OnEventNoArgs {
        get => onEventNoArgs;
        set => onEventNoArgs = value;
    }


    /// <summary>
    /// Initializes a new instance of the EventBinding class.
    /// </summary>
    /// <param name="onEvent"></param>
    public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
    public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

    /// <summary>
    /// Adds an action to be invoked when the event is triggered.
    /// </summary>
    /// <param name="onEvent"></param>
    public void Add(Action onEvent) => onEventNoArgs += onEvent;
    /// <summary>
    /// Removes an action from being invoked when the event is triggered.
    /// </summary>
    /// <param name="onEvent"></param>
    public void Remove(Action onEvent) => onEventNoArgs -= onEvent;

    /// <summary>
    /// Adds an action to be invoked when the event is triggered.
    /// </summary>
    /// <param name="onEvent"></param>
    public void Add(Action<T> onEvent) => this.onEvent += onEvent;

    /// <summary>
    /// Removes an action from being invoked when the event is triggered.
    /// </summary>
    /// <param name="onEvent"></param>
    public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
}
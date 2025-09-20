
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Contains methods and properties related to event buses and event types in the Unity application.
/// </summary>
public static class EventBusUtil {
    public static IReadOnlyList<Type> GlobalEventTypes { get; set; }
    public static IReadOnlyList<Type> GlobalEventBusTypes { get; set; }

    //public static IReadOnlyList<Type> CharacterEventTypes { get; set; }
    //public static IReadOnlyDictionary<Type, LocalEventBus<IEvent>> CharacterEventBuses { get; set; }

#if UNITY_EDITOR
    public static PlayModeStateChange PlayModeState { get; set; }

    /// <summary>
    /// Initializes the Unity Editor related components of the EventBusUtil.
    /// The [InitializeOnLoadMethod] attribute causes this method to be called every time a script
    /// is loaded or when the game enters Play Mode in the Editor. This is useful to initialize
    /// fields or states of the class that are necessary during the editing state that also apply
    /// when the game enters Play Mode.
    /// The method sets up a subscriber to the playModeStateChanged event to allow
    /// actions to be performed when the Editor's play mode changes.
    /// </summary>    
    [InitializeOnLoadMethod]
    public static void InitializeEditor() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state) {
        PlayModeState = state;
        if (state == PlayModeStateChange.ExitingPlayMode) {
            ClearGlobalBuses();
        }
    }
#endif

    /// <summary>
    /// Initializes the EventBusUtil class at runtime before the loading of any scene.
    /// The [RuntimeInitializeOnLoadMethod] attribute instructs Unity to execute this method after
    /// the game has been loaded but before any scene has been loaded, in both Play Mode and after
    /// a Build is run. This guarantees that necessary initialization of bus-related types and events is
    /// done before any game objects, scripts or components have started.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize() {
        GlobalEventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
        GlobalEventBusTypes = InitializeGlobalBuses();

        //CharacterEventTypes = PredefinedAssemblyUtil.GetTypes(typeof(ICharacterEvent));
        //CharacterEventBuses = InitializeLocalBuses(CharacterEventTypes);
    }

    static List<Type> InitializeGlobalBuses() {
        List<Type> eventBusTypes = new List<Type>();

        foreach (var eventType in GlobalEventTypes) {
            var busType = typeof(GlobalEventBus<>).MakeGenericType(eventType);
            eventBusTypes.Add(busType);
            Debug.Log($"Initialized EventBus<{eventType.Name}>");
        }

        return eventBusTypes;
    }

    /*static Dictionary<Type, LocalEventBus<IEvent>> InitializeLocalBuses(IEnumerable<Type> eventTypes) {
        Dictionary<Type, LocalEventBus<IEvent>> eventBuses = new Dictionary<Type, LocalEventBus<IEvent>>();
        foreach (var eventType in eventTypes) {
            var busType = typeof(LocalEventBus<>).MakeGenericType(eventType);
            var busInstance = Activator.CreateInstance(busType, new HashSet<IEventBinding<IEvent>>());
            eventBuses[eventType] = (LocalEventBus<IEvent>)busInstance;
            Debug.Log($"Initialized EventBus<{eventType.Name}>");
        }
        return eventBuses;
    }*/

    /// <summary>
    /// Clears (removes all listeners from) all event buses in the application.
    /// </summary>
    public static void ClearGlobalBuses() {
        Debug.Log("Clearing all buses...");
        for (int i = 0; i < GlobalEventBusTypes.Count; i++) {
            var busType = GlobalEventBusTypes[i];
            var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
            clearMethod?.Invoke(null, null);
        }
    }
}

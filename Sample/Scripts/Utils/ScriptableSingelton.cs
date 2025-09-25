using UnityEngine;

/// <summary>
/// Singleton base class for ScriptableObjects.
/// </summary>
/// <typeparam name="T">The type of the ScriptableSingleton<typeparamref name="T"/>.</typeparam>
public abstract class ScriptableSingleton<T> : ScriptableObject
    where T : ScriptableSingleton<T> {
    private static T _instance;

    /// <summary>
    /// Singleton instance of the ScriptableObject.
    /// </summary>
    public static T Instance {
        get {
            if (_instance == null) {
                T[] instances = Resources.LoadAll<T>("");
                if (instances == null || instances.Length == 0) {
                    Debug.LogError($"[ScriptableSingleton] No instance of {typeof(T).Name} found in Resources folder.");
                }
                else if (instances.Length > 1) {
                    Debug.Log($"[ScriptableSingleton] Multiple({instances.Length}) instances of {typeof(T).Name} found in Resources folder. Using the first one.");
                }
                _instance = instances[0];

            }
            return _instance;
        }
    }

    /// <summary>
    /// Ensures only one instance exists. Logs a warning if another instance is created.
    /// </summary>
    protected virtual void OnEnable() {
        if (_instance == null) {
            _instance = this as T;
        }
        else if (_instance != this) {
            Debug.LogWarning($"[ScriptableSingleton] Another instance of {typeof(T).Name} already exists. This instance will be ignored.");
        }
    }
}
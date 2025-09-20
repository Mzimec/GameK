using UnityEngine;

public abstract class ScriptableSingleton<T> : ScriptableObject
    where T : ScriptableSingleton<T> {
    private static T _instance;
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

    protected virtual void OnEnable() {
        if (_instance == null) {
            _instance = this as T;
        }
        else if (_instance != this) {
            Debug.LogWarning($"[ScriptableSingleton] Another instance of {typeof(T).Name} already exists. This instance will be ignored.");
        }
    }
}
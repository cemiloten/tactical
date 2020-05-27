using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        if (Instance != null) {
            Debug.LogWarning($"Instance of {typeof(T)} already exits, destroying self.");
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }
}

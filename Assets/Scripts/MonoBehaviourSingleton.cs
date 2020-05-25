using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component {
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        if (Instance != null) {
            Debug.LogError($"Instance already exits on object: {Instance.name}");
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this as T;
    }
}
using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected abstract void OnAwake();

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Instance already exits on object: {Instance.name}");
            return;
        }

        Instance = this as T;
        OnAwake();
    }
}

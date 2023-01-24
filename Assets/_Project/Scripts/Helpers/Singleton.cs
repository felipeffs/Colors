using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"There can be only one instance of {name}");
            Destroy(this);
            return;
        }
        Instance = this as T;

        SingletonAwake();
    }

    protected virtual void SingletonAwake()
    {

    }
}
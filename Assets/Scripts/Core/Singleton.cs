using UnityEngine;

// Singleton<T> pattern - generic base for MonoBehaviour singletons.
// Set Persistent to true for cross-scene singletons (DontDestroyOnLoad).
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                return _instance;
            }
        }
    }

    protected virtual bool Persistent => false;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            if (Persistent)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                // Non-persistent: new scene instance replaces the old one
                _instance = this as T;
                return;
            }
        }

        _instance = this as T;

        if (Persistent)
            DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}

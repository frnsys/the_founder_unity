// From: http://wiki.unity3d.com/index.php/Toolbox

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static object _lock = new object();
    protected static T _instance;

    // If the singleton is meant to be created from a prefab, set it here.
    protected static GameObject prefab;

    public static bool hasInstance {
        get { return _instance != null; }
    }

    public static T Instance {
        get {
            if (applicationIsQuitting) {
                Debug.LogWarning("[Singleton Instance '" + typeof(T) + "' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            lock(_lock) {
                if (_instance == null) {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (FindObjectsOfType(typeof(T)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton. Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null) {
                        GameObject singleton;
                        if (prefab == null) {
                            singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                        } else {
                            singleton = Instantiate(prefab) as GameObject;
                            _instance = singleton.GetComponent<T>();
                        }
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) + " is needed in the scene, so " + singleton + " was created with DontDestroyOnLoad.");
                        Debug.Log("Note that this creation does NOT use a prefab, so if you have a particular configuration for the singleton as part of a prefab, you must instead load that yourself.");
                    } else {
                        Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
                    }
                }
                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    // When Unity quits, it destroys objects in a random order.
	// In principle, a Singleton is only destroyed when application quits.
	// If any script calls Instance after it have been destroyed,
	// it will create a buggy ghost object that will stay on the Editor scene
	// even after stopping playing the Application. Really bad!
	// So, this was made to be sure we're not creating that buggy ghost object.
    public void OnDestroy() {
        applicationIsQuitting = true;
    }

    public static void Reset() {
        applicationIsQuitting = false;
    }
}

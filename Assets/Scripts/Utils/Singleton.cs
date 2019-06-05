using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	public static T Instance
	{
		get
		{
			lock(_lock)
			{
				if (applicationIsQuitting) {
					Debug.Log("[Singleton] Instance '"+ typeof(T) +
						"' already destroyed on application quit." +
						" Won't create again - returning null.");
					return null;
				}

				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));

					if ( FindObjectsOfType(typeof(T)).Length > 1 )
					{
						Debug.LogError("[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it.");
						return _instance;
					}

					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						singleton.name = "(singleton) "+ typeof(T).ToString();
						_instance = singleton.AddComponent<T>();
						
						if (!Application.isEditor || Application.isPlaying)
							DontDestroyOnLoad(singleton);
						#if UNITY_EDITOR
						Debug.Log("[Singleton] An instance of " + typeof(T) + 
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
						#endif
					} else {
						Transform t = _instance.gameObject.transform;
						string s = "";
						while( t != null )
						{
							s = t.name + "/" + s;
							t = t.parent;
						}

						Debug.Log("[Singleton] Using instance already created: " + s);
					}
				}

				return _instance;
			}
		}
	}

	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public virtual void OnDestroy () {
		lock(_lock) {
			applicationIsQuitting = true;
		}
	}

	public static bool HasInstance {
		get {
			lock (_lock) {
				return null != _instance && !applicationIsQuitting;
			}
		}
	}

	public static bool IsDestroyed {
		get {
			lock (_lock) {
				return applicationIsQuitting;
			}
		}
	}
}
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T kInstance;
	
	public static void Init()
	{
		if (kInstance == null)
			kInstance = GameObject.FindObjectOfType(typeof(T)) as T;
		
		if (kInstance == null)
		{
			string instance_name = typeof(T).Name;
			GameObject container = new GameObject(instance_name);
			kInstance = container.AddComponent(typeof(T)) as T;
		}
	}

	protected virtual void Awake(){
		if(kInstance != null && kInstance != this)
		{
			Destroy(this.gameObject);
		}
		Init();
	}

	public static T Instance
	{
		get
		{
			if (kInstance == null)
				Init();
			
			return kInstance;
		}
	}
}


using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {

	#region Singleton
	private static ResourceManager instance;
	public static ResourceManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (ResourceManager)FindObjectOfType(typeof(ResourceManager));

				if (instance == null)
				{
					Debug.LogError(typeof(FadeManager) + "is nothing");
				}
			}

			return instance;
		}
	}
	#endregion Singleton

	public void Awake()
	{
		if (this != Instance)
		{
			Destroy(this.gameObject);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

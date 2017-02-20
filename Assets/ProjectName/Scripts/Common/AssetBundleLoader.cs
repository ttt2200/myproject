using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssetBundles;

public class AssetBundleLoader : MonoBehaviour {

	#region Singleton
	private static AssetBundleLoader instance;
	public static AssetBundleLoader Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (AssetBundleLoader)FindObjectOfType(typeof(AssetBundleLoader));

				if (instance == null)
				{
					Debug.LogError(typeof(AssetBundleLoader) + "is nothing");
				}
			}

			return instance;
		}
	}
	#endregion

	public string AssetBundleName = "";
	public string SceneName = "";
	private bool isLoaded_;

	void Awake()
	{
		if (this != Instance)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	public IEnumerator LoadScene()
	{
		yield return StartCoroutine(Initialize());
		
		// Load variant level which depends on variants.
		//yield return StartCoroutine(InitializeLevelAsync(SceneName, true));
		yield return StartCoroutine(InitializeLevelAsync(SceneName, false));
	}
	
	public IEnumerator LoadScene(string assetBundleName, string sceneName)
	{
		AssetBundleName = assetBundleName;
		SceneName = sceneName;

		if (AssetBundleName != "" && SceneName != "")
		{
			yield return StartCoroutine(Initialize());

			// Load variant level which depends on variants.
			//yield return StartCoroutine(InitializeLevelAsync(SceneName, true));
			yield return StartCoroutine(InitializeLevelAsync(SceneName, false));
		}
		else
		{
			Debug.LogError("Don't set AssetBundleName or SceneName");
		}
	}

	// Initialize the downloading url and AssetBundleManifest object.
	protected IEnumerator Initialize()
	{
		// Don't destroy this gameObject as we depend on it to run the loading script.
		DontDestroyOnLoad(gameObject);

		// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
		// (This is very dependent on the production workflow of the project. 
		// 	Another approach would be to make this configurable in the standalone player.)
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		AssetBundleManager.SetDevelopmentAssetBundleServer();
#else
		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
#endif

		// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
		var request = AssetBundleManager.Initialize();

		if (request != null)
			yield return StartCoroutine(request);
	}

	protected IEnumerator InitializeLevelAsync(string levelName, bool isAdditive)
	{
		isLoaded_ = false;
		// This is simply to get the elapsed time for this phase of AssetLoading.
		//float startTime = Time.realtimeSinceStartup;

		// Load level from assetBundle.
		AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(AssetBundleName, levelName, isAdditive);
		if (request == null)
			yield break;
		
		yield return StartCoroutine(request);
		// Calculate and display the elapsed time.
		//float elapsedTime = Time.realtimeSinceStartup - startTime;
		//Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");

		AssetBundleManager.DeleteAssetBundleManager();
		isLoaded_ = true;
	}

	public bool GetIsLoaded()
	{
		return isLoaded_;
	}
}

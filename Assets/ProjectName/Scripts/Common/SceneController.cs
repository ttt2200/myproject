using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
	#region Singleton
	private static SceneController instance;

	public static SceneController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (SceneController)FindObjectOfType(typeof(SceneController));

				if (instance == null)
				{
					Debug.LogError(typeof(SceneController) + "is nothing");
				}
			}

			return instance;
		}
	}
	#endregion Singleton

	public delegate void CallBack();
	private AsyncOperation async_;
	private bool isLoad_ = false;
	private float time_ = 0.0f;
	private	FadeManager fadeManager_;
	private LoadManager loadManager_;
	private AssetBundleLoader assetBundleLoader_;
	private const float defaultInterval_ = 0.75f;

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
	void Start()
	{
		if (fadeManager_ == null)
			fadeManager_ = FadeManager.Instance;

		if (loadManager_ == null)
			loadManager_ = LoadManager.Instance;

		if (assetBundleLoader_ == null)
			assetBundleLoader_ = AssetBundleLoader.Instance;
	}

	// Update is called once per frame
	void Update()
	{
		time_ += Time.deltaTime;
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	/// <summary>
	/// シーン遷移(演出付き)
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <returns></returns>
	public void StartLoadScene(string sceneName)
	{
		StartLoadScene(sceneName, 0.0f, null);
	}

	/// <summary>
	/// シーン遷移(演出付き)
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartLoadScene(string sceneName, float interval, CallBack func = null)
	{
		// ロード中の場合実行しない
		if (isLoad_ || fadeManager_.GetIsFade()) return;
		if (interval <= 0.0f) interval = defaultInterval_;
		//SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);

		fadeManager_.SetIsSimpleFade(false);
		fadeManager_.StartFadeIn(interval, null);
		StartCoroutine(LoadScene(sceneName, interval, () => {
			fadeManager_.StartFadeOut(interval, null);
			//SceneManager.UnloadScene("LoadScene");
		}));

		if (func != null) func();
	}

	/// <summary>
	/// AssetBundleからのシーン遷移(演出付き)
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <returns></returns>
	public void StartLoadSceneForAssetBundle(string assetBundleName, string sceneName)
	{
		StartLoadSceneForAssetBundle(assetBundleName, sceneName, 0.0f, null);
	}

	/// <summary>
	/// AssetBundleからのシーン遷移(演出付き)
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartLoadSceneForAssetBundle(string assetBundleName, string sceneName, float interval, CallBack func = null)
	{
		// ロード中の場合実行しない
		if (isLoad_ || fadeManager_.GetIsFade()) return;
		if (interval <= 0.0f) interval = defaultInterval_;
		//SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);

		fadeManager_.SetIsSimpleFade(false);
		fadeManager_.StartFadeIn(interval, null);
		StartCoroutine(LoadSceneForAssetBundle(assetBundleName, sceneName, interval, () => {
			fadeManager_.StartFadeOut(interval, null);
			//SceneManager.UnloadScene("LoadScene");
		}));

		if (func != null) func();
	}

	/// <summary>
	/// シーン遷移(簡易フェードのみ)
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartFadeLoadScene(string sceneName)
	{
		StartFadeLoadScene(sceneName, 0.0f, null);
	}

	/// <summary>
	/// シーン遷移(簡易フェードのみ)
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartFadeLoadScene(string sceneName, float interval, CallBack func = null)
	{
		// ロード中の場合実行しない
		if (isLoad_ || fadeManager_.GetIsFade()) return;
		if (interval <= 0.0f) interval = defaultInterval_;
		//SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);

		fadeManager_.SetIsSimpleFade(true);
		fadeManager_.StartFadeIn(interval, null);
		StartCoroutine(LoadScene(sceneName, interval, () => {
			fadeManager_.StartFadeOut(interval, null);
			//SceneManager.UnloadScene("LoadScene");
		}));

		if (func != null) func();
	}

	/// <summary>
	/// AssetBundleからのシーン遷移(簡易フェードのみ)
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <returns></returns>
	public void StartFadeLoadSceneForAssetBundle(string assetBundleName, string sceneName)
	{
		StartFadeLoadSceneForAssetBundle(assetBundleName, sceneName, 0.0f, null);
	}

	/// <summary>
	/// AssetBundleからのシーン遷移(簡易フェードのみ)
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartFadeLoadSceneForAssetBundle(string assetBundleName, string sceneName, float interval, CallBack func = null)
	{
		// ロード中の場合実行しない
		if (isLoad_ || fadeManager_.GetIsFade()) return;
		if (interval <= 0.0f) interval = defaultInterval_;
		//SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);

		fadeManager_.SetIsSimpleFade(true);
		fadeManager_.StartFadeIn(interval, null);
		StartCoroutine(LoadSceneForAssetBundle(assetBundleName, sceneName, interval, () => {
			fadeManager_.StartFadeOut(interval, null);
			//SceneManager.UnloadScene("LoadScene");
		}));

		if (func != null) func();
	}

	/// <summary>
	/// シーン遷移(簡易ポップアップ表示のみ)
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <returns></returns>
	public void StartSimpleLoadScene(string sceneName)
	{
		StartSimpleLoadScene(sceneName, 0.0f, null);
	}

	/// <summary>
	/// シーン遷移(簡易ポップアップ表示のみ)
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartSimpleLoadScene(string sceneName, float interval, CallBack func = null)
	{
		if (isLoad_) return;
		if (interval <= 0.0f) interval = defaultInterval_;
		
		loadManager_.StartLoad(interval);
		StartCoroutine(LoadScene(sceneName, interval, () => {
			loadManager_.DeleteLoadCanvas();
		}));
		
		if (func != null) func();
	}

	/// <summary>
	/// AssetBundleからのシーン遷移(簡易ポップアップ表示のみ)
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <returns></returns>
	public void StartSimpleLoadSceneForAssetBundle(string assetBundleName, string sceneName)
	{
		StartSimpleLoadSceneForAssetBundle(assetBundleName, sceneName, 0.0f, null);
	}

	/// <summary>
	/// AssetBundleからのシーン遷移(簡易ポップアップ表示のみ)
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	public void StartSimpleLoadSceneForAssetBundle(string assetBundleName, string sceneName, float interval, CallBack func = null)
	{
		if (isLoad_) return;
		if (interval <= 0.0f) interval = defaultInterval_;

		loadManager_.StartLoad(interval);
		StartCoroutine(LoadSceneForAssetBundle(assetBundleName, sceneName, interval, () => {
			loadManager_.DeleteLoadCanvas();
		}));

		if (func != null) func();
	}

	/// <summary>
	/// シーン遷移処理
	/// </summary>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	private IEnumerator LoadScene(string sceneName, float interval, CallBack func)
	{
		isLoad_ = true;
		time_ = 0.0f;

		// 非同期でロード開始
		async_ = SceneManager.LoadSceneAsync(sceneName);
		// デフォルトはtrue。ロード完了したら勝手にシーンきりかえ発生しないよう設定。
		async_.allowSceneActivation = false;
		
		// 非同期読み込み中の処理
		while (!async_.isDone)
		{
			time_ += Time.deltaTime;

			// ロードが完了していないことを伝える
			fadeManager_.SetIsSceneLoad(false);
			loadManager_.SetIsSceneLoad(false);

			// ロード完了
			if (async_.progress >= 0.9f && time_ >= interval)
			{
				isLoad_ = false;
				fadeManager_.SetIsSceneLoad(true);
				loadManager_.SetIsSceneLoad(true);

				while (fadeManager_.GetIsFade() || loadManager_.GetIsLoad())
				{
					//Debug.Log("isload : " + loadManager_.GetIsLoad());
					yield return null;
				}
				SceneManager.UnloadScene(sceneName);
				async_.allowSceneActivation = true;
			}
			yield return null;
		}

		while (fadeManager_.GetIsFade() || loadManager_.GetIsLoad())
		{
			yield return null;
		}
		if (func != null) func();
	}

	/// <summary>
	/// AssetBundleからのシーン遷移処理
	/// </summary>
	/// <param name="assetBundleName">AssetBundle名称</param>
	/// <param name="sceneName">シーン名称</param>
	/// <param name="interval">遷移にかかる最低時間</param>
	/// <param name="func">コールバック関数</param>
	/// <returns></returns>
	private IEnumerator LoadSceneForAssetBundle(string assetBundleName, string sceneName, float interval, CallBack func)
	{
		isLoad_ = true;
		time_ = 0.0f;

		// 非同期でロード開始
		//async_ = SceneManager.LoadSceneAsync(sceneName);
		// デフォルトはtrue。ロード完了したら勝手にシーンきりかえ発生しないよう設定。

		// 非同期読み込み中の処理
		while (isLoad_)
		{
			time_ += Time.deltaTime;

			// ロードが完了していないことを伝える
			fadeManager_.SetIsSceneLoad(false);
			loadManager_.SetIsSceneLoad(false);

			// 画面が完全に暗転してからAssetBundleからシーンを読み込む
			if (time_ >= interval)
			{
				StartCoroutine(assetBundleLoader_.LoadScene(assetBundleName, sceneName));

				fadeManager_.SetIsSceneLoad(true);
				loadManager_.SetIsSceneLoad(true);
				while (!assetBundleLoader_.GetIsLoaded())
				{
					//Debug.Log("isload : " + loadManager_.GetIsLoad());
					yield return null;
				}
				isLoad_ = false;
			}
			yield return null;
		}

		while (fadeManager_.GetIsFade() || loadManager_.GetIsLoad())
		{
			yield return null;
		}
		if (func != null) func();
	}

}

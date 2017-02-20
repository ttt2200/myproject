using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour {

	#region Singleton
	private static FadeManager instance;

	public static FadeManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (FadeManager)FindObjectOfType(typeof(FadeManager));

				if (instance == null)
				{
					Debug.LogError(typeof(FadeManager) + "is nothing");
				}
			}

			return instance;
		}
	}
	#endregion Singleton
	
	public delegate void CallBack();
	public int layer_ = 1;
	public const float defaultInterval = 0.25f;
	public enum FadeState : int
	{
		NONE,
		FadeIn,
		FadeOut
	}

	private bool isFade_ = false;
	private bool isSimpleFade_ = false;
	private bool isSceneLoaded_ = false;
	private FadeState fadeState_ = 0;
	private float fadeAlpha_ = 0.0f;
	private GameObject fadeCamera_ = null;
	private GameObject fadeCanvas_ = null;
	private GameObject fadeImage_ = null;
	private GameObject loadText_ = null;
	private GameObject particle_ = null;
	
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
		if (fadeCanvas_ != null && fadeImage_ != null)
		{
			Image img = fadeImage_.GetComponent<Image>();
			img.color = new Color(img.color.r, img.color.g, img.color.b, this.fadeAlpha_);

			updateLoadScene();
		}
	}

	/// <summary>
	/// フェード画面の生成
	/// </summary>
	/// <returns></returns>
	private void createFadeCanvas()
	{
		// キャンバスは1つだけ
		if (fadeCanvas_ != null) return;
		Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
		
		// プレハブからインスタンスを生成
		fadeCamera_ = Instantiate(Resources.Load("Prefabs/FadeCamera"), position, Quaternion.identity) as GameObject;
		fadeCanvas_ = Instantiate(Resources.Load("Prefabs/FadeCanvas"), position, Quaternion.identity) as GameObject;
		fadeCanvas_.GetComponent<Canvas>().worldCamera = fadeCamera_.GetComponent<Camera>();
		fadeImage_ = fadeCanvas_.transform.FindChild("FadeImage").gameObject;
		loadText_ = fadeCanvas_.transform.FindChild("LoadText").gameObject;
		particle_ = fadeCanvas_.transform.FindChild("Particle").gameObject;

		DontDestroyOnLoad(fadeCamera_);
		DontDestroyOnLoad(fadeCanvas_);
	}

	/// <summary>
	/// フェード画面の破棄
	/// </summary>
	/// <returns></returns>
	private void deleteFadeCanvas()
	{
		if (fadeCamera_ != null) Destroy(fadeCamera_);
		if (fadeCanvas_ != null) Destroy(fadeCanvas_);
		if (fadeImage_ != null) Destroy(fadeImage_);
		if (loadText_ != null) Destroy(loadText_);
		if (particle_ != null) Destroy(particle_);
	}

	/// <summary>
	/// フェード開始
	/// </summary>
	/// <param name="sceneName">遷移先のシーン名称</param>
	/// <returns></returns>
	public void StartFade(string sceneName) {
		StartFade(sceneName, 0.0f);
	}

	/// <summary>
	/// フェード開始
	/// </summary>
	/// <param name="sceneName">遷移先のシーン名称</param>
	/// <param name="interval">遷移にかかる時間</param>
	/// <returns></returns>
	public void StartFade(string sceneName, float interval) {
		if (!this.isFade_)
		{
			createFadeCanvas();
			StartCoroutine(onFade(sceneName, interval));
		}
	}

	/// <summary>
	/// フェードイン開始
	/// </summary>
	/// <param name="interval">遷移にかかる時間</param>
	/// <returns></returns>
	public void StartFadeIn(float interval)
	{
		StartFadeIn(interval, null);
	}

	/// <summary>
	/// フェードイン処理呼び出し
	/// </summary>
	/// <param name="interval">遷移にかかる時間</param>
	/// <param name="func">フェードイン完了後に実行される処理</param>
	/// <returns></returns>
	public void StartFadeIn(float interval, CallBack func = null)
	{
		if (!this.isFade_)
		{
			createFadeCanvas();
			StartCoroutine(onFadeIn(interval, func));
		}
	}

	/// <summary>
	/// フェードアウト処理呼び出し
	/// </summary>
	/// <param name="interval">遷移にかかる時間</param>
	/// <param name="func">フェードアウト完了後に実行される処理</param>
	/// <returns></returns>
	public void StartFadeOut(float interval, CallBack func = null)
	{
		if (!this.isFade_)
		{
			createFadeCanvas();
			StartCoroutine(onFadeOut(interval, ()=> {
				deleteFadeCanvas();
			}));
		}
	}

	/// <summary>
	/// 高速フェード完了
	/// </summary>
	/// <returns></returns>
	public void QuickFade() {
		switch (this.fadeState_)
		{
			case FadeState.FadeIn:
				this.fadeAlpha_ = 1.0f;
				break;
			case FadeState.FadeOut:
				this.fadeAlpha_ = 0.0f;
				break;
			default:
				Debug.Log("Error QuickFade");
				break;
		}
	}

	/// <summary>
	/// フェードイン処理
	/// </summary>
	/// <param name="interval">遷移にかかる時間</param>
	/// <param name="func">フェードイン完了後に実行される処理</param>
	/// <returns></returns>
	private IEnumerator onFadeIn(float interval, CallBack func = null) {
		// フェード開始
		this.isSceneLoaded_ = true;
		this.isFade_ = true;
		if (interval <= 0.0f) interval = defaultInterval;

		// 指定の時間に合わせてだんだん暗く
		this.fadeState_ = FadeState.FadeIn;
		float elapsedTime = 0.0f;
		this.fadeAlpha_ = 0.0f;
		while ((this.fadeAlpha_ < 1.0f || elapsedTime < interval))
		{
			elapsedTime += Time.deltaTime;
			this.fadeAlpha_ = 1.0f / defaultInterval * elapsedTime;
			checkFadeAlphaValue();
			yield return 0;
		}

		while (!isSceneLoaded_) yield return null;

		// フェード終了
		this.isFade_ = false;
		//if (func != null) func();
	}
	
	/// <summary>
	/// フェードアウト処理
	/// </summary>
	/// <param name="interval">遷移にかかる時間</param>
	/// <param name="func">フェードアウト完了後に実行される処理</param>
	/// <returns></returns>
	private IEnumerator onFadeOut(float interval, CallBack func = null)
	{
		// フェード開始
		this.isFade_ = true;
		if (interval <= 0.0f) interval = defaultInterval;

		// 指定の時間に合わせてだんだん明るく
		this.fadeState_ = FadeState.FadeOut;
		float elapsedTime = 0.0f;
		this.fadeAlpha_ = 1.0f;
		//while (this.fadeAlpha_ > 0.0f || elapsedTime < interval)
		while (this.fadeAlpha_ > 0.0f)
			{
			elapsedTime += Time.deltaTime;
			this.fadeAlpha_ = 1.0f - 1.0f / defaultInterval * elapsedTime;
			checkFadeAlphaValue();
			yield return 0;
		}

		if (func != null) func();

		// フェード終了
		this.isFade_ = false;
	}
	
	/// <summary>
	/// フェード処理
	/// </summary>
	/// <param name="sceneName">遷移先のシーン名称</param>
	/// <param name="interval">遷移にかかる時間</param>
	/// <returns></returns>
	private IEnumerator onFade(string sceneName, float interval)
	{
		// フェード開始
		this.isFade_ = true;

		// 指定の時間に合わせてだんだん暗く
		this.fadeState_ = FadeState.FadeIn;
		float elapsedTime = 0.0f;
		this.fadeAlpha_ = 0.0f;
		while (this.fadeAlpha_ < 1.0f || elapsedTime < interval) {
			elapsedTime += Time.deltaTime;
			if (interval <= 0.0f)
			{
				this.fadeAlpha_ = 1.0f / defaultInterval * elapsedTime;
			}
			else
			{
				this.fadeAlpha_ = 1.0f / interval * elapsedTime;
			}
			yield return 0;
		}

		// シーンを移動したり？
		SceneManager.LoadScene(sceneName);

		// 指定の時間に合わせてだんだん明るく
		this.fadeState_ = FadeState.FadeOut;
		elapsedTime = 0.0f;
		this.fadeAlpha_ = 1.0f;
		while (this.fadeAlpha_ > 0.0f || elapsedTime < interval)
		{
			elapsedTime += Time.deltaTime;
			if (interval <= 0.0f)
			{
				this.fadeAlpha_ = 1.0f - 1.0f / defaultInterval * elapsedTime;
			}
			else
			{
				this.fadeAlpha_ = 1.0f - 1.0f / interval * elapsedTime;
			}
			checkFadeAlphaValue();
			yield return 0;
		}

		// フェード終了
		this.isFade_ = false;
		yield return new WaitForSeconds(0.1f);
	}

	/// <summary>
	/// ローディング画面更新
	/// </summary>
	/// <returns></returns>
	private void updateLoadScene()
	{
		if (loadText_ == null) return;
		if (particle_ == null) return;

		if (isSimpleFade_)
		{
			loadText_.SetActive(false);
			particle_.SetActive(false);
			return;
		}

		switch (this.fadeState_)
		{
			case FadeState.FadeIn:
				loadText_.SetActive(true);
				particle_.SetActive(true);
				break;
			case FadeState.FadeOut:
				loadText_.SetActive(false);
				particle_.SetActive(false);
				break;
			default:
				Debug.Log("Error UpdateLoadText");
				break;
		}
	}

	/// <summary>
	/// アルファ値の確認
	/// </summary>
	/// <returns></returns>
	private void checkFadeAlphaValue()
	{
		if (fadeAlpha_ >= 1.0f)
		{
			fadeAlpha_ = 1.0f;
		}
		else if (fadeAlpha_ <= 0.0f)
		{
			fadeAlpha_ = 0.0f;
		}
	}

	/// <summary>
	/// フェードフラグ取得
	/// </summary>
	/// <returns>フェードフラグ</returns>
	public bool GetIsFade()
	{
		return isFade_;
	}
	
	/// <summary>
	/// シーン遷移状態の設定
	/// </summary>
	/// <param name="isTrue"></param>
	/// <returns></returns>
	public void SetIsSceneLoad(bool isTrue)
	{
		isSceneLoaded_ = isTrue;
	}

	/// <summary>
	/// 簡易フェードフラグの設定
	/// </summary>
	/// <param name="isTrue"></param>
	/// <returns></returns>
	public void SetIsSimpleFade(bool isTrue)
	{
		isSimpleFade_ = isTrue;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadManager : MonoBehaviour
{
	#region Singleton
	private static LoadManager instance;

	public static LoadManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (LoadManager)FindObjectOfType(typeof(LoadManager));

				if (instance == null)
				{
					Debug.LogError(typeof(LoadManager) + "is nothing");
				}
			}

			return instance;
		}
	}
	#endregion Singleton

	public delegate void CallBack();
	private GameObject fadeCamera_ = null;
	private GameObject loadCanvas_ = null;
	private GameObject loadPanel_ = null;
	private Text loadText_ = null;
	private bool isLoad_ = false;
	private bool isUpdateText_ = false;
	private bool isSceneLoaded_ = false;
	private int addCount_ = 0;
	private float frame_ = 0.0f;
	private string baseText_ = "";
	private string addText_ = "";

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
	}

	// Update is called once per frame
	void Update()
	{
		if (loadCanvas_ != null)
		{
			updateLoadText();
		}
	}
	/// <summary>
	/// ロード処理呼び出し
	/// </summary>
	/// <returns></returns>
	public void StartLoad()
	{
		StartLoad(0.0f);
	}

	/// <summary>
	/// ロード処理呼び出し
	/// </summary>
	/// <param name="interval"></param>
	/// <returns></returns>
	public void StartLoad(float interval)
	{
		isUpdateText_ = true;

		CreateLoadCanvas();
		StartCoroutine(onLoad(interval));
	}

	/// <summary>
	/// ロードキャンバスの生成
	/// </summary>
	/// <returns></returns>
	public void CreateLoadCanvas()
	{
		if (loadCanvas_ != null) return;
		Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);

		fadeCamera_ = Instantiate(Resources.Load("Prefabs/FadeCamera"), position, Quaternion.identity) as GameObject;
		loadCanvas_ = Instantiate(Resources.Load("Prefabs/Loadcanvas"), position, Quaternion.identity) as GameObject;
		loadPanel_ = loadCanvas_.transform.FindChild("Panel").gameObject;
		loadText_ = loadPanel_.transform.FindChild("LoadText").GetComponent<Text>();
	}

	/// <summary>
	/// ロードキャンバスの破棄
	/// </summary>
	/// <returns></returns>
	public void DeleteLoadCanvas()
	{
		if (fadeCamera_ != null) Destroy(fadeCamera_);
		if (loadCanvas_ != null) Destroy(loadCanvas_);
		if (loadPanel_ != null) Destroy(loadPanel_);
		if (loadText_ != null) Destroy(loadText_);
	}

	/// <summary>
	/// ロード文字の更新
	/// </summary>
	/// <returns></returns>
	private void updateLoadText()
	{
		if (loadText_ == null) return;
		if (!isUpdateText_) return;
		StartCoroutine(delayMethod(0.125f, ()=> {
			baseText_ = "Now Loading";
			if (addCount_ >= 3) {
				addCount_ = 0;
				addText_ = "";
			}
			addText_ += " .";
			addCount_++;
			if (loadText_ != null) loadText_.text = baseText_ + addText_;

			isUpdateText_ = true;
		}));

		isUpdateText_ = false;
	}

	/// <summary>
	/// 渡された処理を指定時間後に実行する
	/// </summary>
	/// <param name="delayFrameCount"></param>
	/// <param name="func">実行したい処理</param>
	/// <returns></returns>
	private IEnumerator delayMethod(float delayFrameCount, CallBack func = null)
	{
		if (func == null)
		{
			Debug.Log("Error delayMethod not set function");
		}

		frame_ = 0.0f;
		while(frame_ <= delayFrameCount)
		{
			frame_ += Time.deltaTime;
			yield return null;
		}
		
		if (func != null) func();
	}
	
	/// <summary>
	/// ロード画面処理
	/// </summary>
	/// <param name="interval"></param>
	/// <returns></returns>
	private IEnumerator onLoad(float interval)
	{
		float frame = 0.0f;
		isLoad_ = true;
		isSceneLoaded_ = true;

		if (interval == 0.0f) interval = defaultInterval_;
		while (frame <= interval)
		{
			frame += Time.deltaTime;
			yield return null;
		}

		while (!isSceneLoaded_) yield return null;
		
		isLoad_ = false;
	}

	/// <summary>
	/// ロードフラグ取得
	/// </summary>
	/// <returns>ロードフラグ</returns>
	public bool GetIsLoad()
	{
		return isLoad_;
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
}


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleController : MonoBehaviour
{

	public Button GameStartButton;

	// Use this for initialization
	void Start()
	{
		SceneController sceneController = SceneController.Instance;

		GameStartButton.onClick.AddListener(()=> {
			//sceneController.StartSimpleLoadScene("GameScene");
			sceneController.StartLoadSceneForAssetBundle("gamescene", "GameScene");
		});
	}

	// Update is called once per frame
	void Update()
	{

	}
}

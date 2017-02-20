using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	private SceneController sceneController;
		
	// Use this for initialization
	void Start () {
		if (sceneController == null)
		{
			sceneController = SceneController.Instance;
		}
		Button button = GameObject.Find("GoTitleButton").GetComponent<Button>();
		button.onClick.AddListener(() => {
			sceneController.StartLoadScene("TitleScene");
		});

		button = GameObject.Find("GoGameOver").GetComponent<Button>();
		button.onClick.AddListener(() => {
			GoGameOverScene();
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void GoGameOverScene()
	{
		sceneController.StartFadeLoadSceneForAssetBundle("gameoverscene", "GameOverScene");
	}
}

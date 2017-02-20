using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverController : MonoBehaviour {

	public Button button;
	
	// Use this for initialization
	void Start () {
		SceneController sceneController = SceneController.Instance;

		button.onClick.AddListener(()=> {
			sceneController.StartFadeLoadScene("TitleScene");
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceLoader : ResourceManager {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Sprite LoadSprite(string fileName)
	{
		return Resources.Load<Sprite>("Texture/" + fileName);
	}
	
	public AudioClip LoadSoundSE(string fileName)
	{
		return Resources.Load<AudioClip>("Sound/SE/" + fileName);
	}
}

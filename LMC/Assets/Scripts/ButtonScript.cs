using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoToMenu () {
		Application.LoadLevel(0);
	}
	public void GoToGame () {
        PlayerPrefs.SetString("currentScriptText", "404");
		Application.LoadLevel (1);
	}
	public void GoToLoad () {
		Application.LoadLevel(2);
	}
	public void GoToOptions () {
		Application.LoadLevel(3);
	}
	public void GoToTutorial () {
		Application.LoadLevel(4);
	}
	public void GoToExit () {
		Application.Quit();
	}



}

using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void GoToGame () {
		Application.LoadLevel(0);
	}
	public void GoToOptions () {
		Application.LoadLevel(1);
	}
	public void GoToLoad () {
		Application.LoadLevel(2);
	}
	public void GoToMenu () {
		Application.LoadLevel(3);
	}
}

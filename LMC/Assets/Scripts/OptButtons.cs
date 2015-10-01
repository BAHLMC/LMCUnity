using UnityEngine;
using System.Collections;

public class OptButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void GoToGame () {
		Application.LoadLevel(1);
	}

    public void GoToMainMenu()
    {
        Application.LoadLevel(0);
    }

    public void QualityLow()
    {
        
    }

    public void QualityMed()
    {
        
    }

    public void QualityHigh()
    {
        
    }
}

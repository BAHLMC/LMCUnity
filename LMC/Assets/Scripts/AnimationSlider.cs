using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationSlider : MonoBehaviour {

    public GameObject AnimSlider;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey ("Animation Speed"))
			AnimSlider.GetComponent<Slider> ().value = PlayerPrefs.GetFloat ("Animation Speed");
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.GetChild(1).GetComponent<Text>().text = AnimSlider.GetComponent<Slider>().value + "";
       
	}

    public void setPlayerPref()
    {
        PlayerPrefs.SetFloat("Animation Speed", AnimSlider.GetComponent<Slider>().value);
    }
}

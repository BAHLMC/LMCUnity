using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationSlider : MonoBehaviour {

	public GameObject AnimSlider;
	public InputField AnimText;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey ("Animation Speed"))
			AnimSlider.GetComponent<Slider> ().value = PlayerPrefs.GetFloat ("Animation Speed");
	}

    public void setPlayerPref()
    {
        PlayerPrefs.SetFloat("Animation Speed", AnimSlider.GetComponent<Slider>().value);
		AnimText.text = AnimSlider.GetComponent<Slider>().value + "";
	}
}

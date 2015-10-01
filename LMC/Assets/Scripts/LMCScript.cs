using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LMCScript : MonoBehaviour {

    // Private booleans for switching between modes
    private bool isRunning;
    private bool inEditMode;

    //Interactive variables set in Unity
    public Text parsedTextBox;
    public InputField scriptInput;

	// Use this for initialization
	void Start () {
        parsedTextBox = GetComponent<Text>();
        scriptInput = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update () {
        scriptInput.enabled = inEditMode;
	}

    void fixedUpdate()
    {
        if (isRunning)
            doNextStep();
    }

    void doNextStep()
    {

    }


    public void onEditClicked()
    {
        inEditMode = true;
    }

    public void onSaveClicked()
    {
        inEditMode = false;
        // updateParsedText;
    }

    public void onPlayPauseClicked()
    {
        isRunning = !isRunning;
    }

    public void onRunClicked()
    {
        isRunning = true;
    }

    public void onResetClicked()
    {
        isRunning = false;
    }

    public void onStepClicked()
    {
        isRunning = false;
        doNextStep();
    }



}

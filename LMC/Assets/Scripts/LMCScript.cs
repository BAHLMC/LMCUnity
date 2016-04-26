using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class LMCScript : MonoBehaviour {

    // Error Variables
    public GUISkin errorGUISkin;
    private bool needsReset = false;
    private bool errorFound = false;
    private int errorCode = -1;
    private String[] errorStrings = new string[] {
        "No HALT command found. The program must contain at least one halt.",
        "Invalid command in the script. Please confirm that all commands you listed are accepted.",
        "You have entered too many commands. This app allows a max of 100 commands. Please shorten your code.",
        "The accumulator has gotten too large. There is a maximum of 3 digits allowed for this app. Please press reset and rerun your code.",
        "You tried to branch to a register that is not allowed. This could be either a data register or a register after your last data register. Please checek your code and then press reset before recompiling.",
        "Please compile your code before attempting to run it. Compile can be found in the bottom left of the screen."
    };


	// HelpBox
	public GUISkin helpGUISkin;
	private bool showHelpBox = false;
	public Texture helpImage;

    // Private booleans for switching between modes
    private bool isRunning = false;
    private bool waitingOnInput = false;
    private bool inEditMode = true;

    //Variables needed for parsing and running the op codes
    private int currentCode;
    private string[] opCodes;
    private bool isNegative;

    //Variables used for the registers
    private int[] registers;
    private GameObject[] pRegisters;

    //Color presets
    private Color usedRegisterBlue = new Color32(0, 207, 255, 255);
    private Color currentOpRegCodes = new Color32(132, 249, 132, 255);
    private Color inputBoxHighlight = new Color32(255, 252, 137, 255);
    private Color defaultRegister = new Color32(223, 253, 255, 255);
    private int prevCode = 0;
    private int prevMem = 0;

    //Animation Stuff
    private float animTime;
    private float lerpTime = 0.05f;
    private bool isAnimating = false;

    //Interactive variables set in Unity
    public Text parsedTextBox;
    public InputField scriptInput;
    public InputField outputField;
    public InputField accumulator;
    public InputField programCounter;
    public InputField inputTextField;
    public GameObject registerPanel;
    public GameObject registerPrefab;
    public InputField instructionRegister;
    public InputField memoryAddressRegister;
    public InputField memoryDataRegister;
    public float autoRunDelay = 2.0f;
    public GameObject animationPrefab;
    public GameObject bgPanel;

    // Use this for initialization
    void Start () {
        registers = new int[100];
        pRegisters = new GameObject[100];
        clearAll();
        if (PlayerPrefs.HasKey("Animation Speed"))
            autoRunDelay = PlayerPrefs.GetFloat("Animation Speed");
        animTime = autoRunDelay/2;
        //Test
        parsedTextBox.text = "The parsed text from the script will go here";
        accumulator.text = "0";

        string tempString = "";
        string scriptText = PlayerPrefs.GetString("currentScriptText", "404");
        if(scriptText == "404")
        {
            scriptInput.text = tempString;
        }
        else
        {
            scriptInput.text = scriptText;
        }

        opCodes = new string[0];
        currentCode = -1;

        float regWidth = registerPanel.GetComponent<RectTransform>().rect.width / 10;
        float regHeight = registerPanel.GetComponent<RectTransform>().rect.height / 10;

        registerPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(regWidth, regHeight);

        for (int dx = 0; dx < 10; ++dx){
            for (int dy = 0; dy < 10; ++dy)
            {
               GameObject reg = Instantiate(registerPrefab) as GameObject;
                reg.transform.SetParent(registerPanel.transform, false);
                pRegisters[10 * dx + dy] = reg;
                reg.transform.GetChild(0).GetComponent<Text>().text = 10 * dx + dy + "";

            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        scriptInput.enabled = inEditMode;

        for (int x = 0; x < 100; ++x)
        {
            if (registers[x] != 0)
                pRegisters[x].transform.GetChild(1).GetComponent<Text>().text = registers[x] + "";
            else
                pRegisters[x].transform.GetChild(1).GetComponent<Text>().text = "000";
        }
        PlayerPrefs.SetString("currentScriptText", scriptInput.text);

    }

    private void clearAll()
    {
        outputField.text = "";
        accumulator.text = "";
        resetInputField();
    }

    private void resetInputField()
    {
        inputTextField.GetComponent<Image>().color = Color.white;
        inputTextField.text = "";
    }

    private bool hasErrors()
    {
        /*
        *

        "No HALT command found. The program must contain at least one halt.",
        "Invalid command in the script. Please confirm that all commands you listed are accepted.",
        "You have entered too many commands. This app allows a max of 100 commands. Please shorten your code.",
        "The accumulator has gotten too large. There is a maximum of 3 digits allowed for this app",
        "You tried to branch to a register that is not allowed. This could be either a data register or a register after your last data register. Please checek your code.",
        "Please compile your code before attempting to run it. Compile can be found in the bottom left of the screen."

        *
        */

        if (!parsedTextBox.text.Contains("000")) {
            foundError(0);
            return true;
        }
        else if (parsedTextBox.text.Contains("-1"))
        {
            foundError(1);
            return true;
        }
        else if (accumulator.text != "" && Int32.Parse(accumulator.text) > 1000 )
        {
            foundError(3);
            return true;
        } else if (parsedTextBox.text.Length == 0)
        {
            foundError(5);
            return true;
        }
        else
        {
            return needsReset;
        }
    }

    void doNextStep()
    {
        Debug.Log("Doing Next Step");
        if (hasErrors() || currentCode > opCodes.Length || currentCode < 0)
        {
            Debug.Log("leaving, hasErrors = " + hasErrors() + "currentCode = " + currentCode);
            isRunning = false;
            return;
        }
        int code;
        try
        {
            code = Int32.Parse(opCodes[currentCode]);
        }
        catch (ArgumentNullException)
        {
            code = -1;
        }
        if (code != -1)
        {
            //update register colors
            Debug.Log("update register colors");
            pRegisters[prevCode].GetComponent<Image>().color = usedRegisterBlue;
            pRegisters[prevMem].GetComponent<Image>().color = usedRegisterBlue;
            pRegisters[currentCode].GetComponent<Image>().color = currentOpRegCodes;
            prevCode = currentCode;

            // int code = Int32.Parse(opCodes[currentCode]);

            //more register colors and CPU text
            Debug.Log("more register colors and CPU text");
            instructionRegister.text = code + "";
            programCounter.text = currentCode + "";
            memoryAddressRegister.text = "";
            memoryDataRegister.text = "";
            if (code < 900 && code != 0)
            {
                pRegisters[code % 100].GetComponent<Image>().color = currentOpRegCodes;
                memoryAddressRegister.text = code % 100 + "";
                memoryDataRegister.text = registers[code % 100] + "";
            }
            prevMem = code % 100;


            Debug.Log("going to precoss code: " + code);

            if (code == 902)
            {
                sendToOutput();
            }
            else if (code == 901)
            {
                getInput();
            }
            else if (code >= 600)
            {
                checkForBranch(code);
            }
            else if (code >= 500)
            {
                loadOp(code);
            }
            else if (code >= 300)
            {
                storeOp(code);
            }
            else if (code >= 200)
            {
                subOp(code);
            }
            else if (code >= 100)
            {
                addOp(code);
            }
            else if (code == 000)
            {
                //halt
                Debug.Log("HLT");
                isRunning = false;
                return;
            }
        }
        currentCode++;
        if (isRunning && !waitingOnInput)
        {
            //Should pause between steps to let animations happen
            StartCoroutine(Delay());
        }
        else
        {
            Debug.Log("isRunning = " + isRunning + ", waitingOnInput = " + waitingOnInput);
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(autoRunDelay);
        doNextStep();
    }



    public void onEditClicked()
    {
        inEditMode = true;
    }

    public void onCompileClicked()
    {
        inEditMode = false;
        String curText = scriptInput.text;
        string[] input = curText.Split('\n');
        string[] result = LoadScript.callStartScanTest(input);
        if (result.Length > 100) {
            foundError(3);
            return;
        }
        String newParse = "";
        for (int i = 0; i < result.Length; i++)
        {
            newParse += result[i] + "\n";
        }
        parsedTextBox.text = newParse;

        opCodes = result;

        for (int x = 0; x < opCodes.Length; ++x)
        {
            try
            {
                registers[x] = Int32.Parse(opCodes[x]);
            }
            catch (ArgumentNullException)
            {
                continue;
            }
        }

        currentCode = 0;

    }

    public void parseInput()
    {
        String input = inputTextField.text;
        if (input.Length > 0)
        {

            StartCoroutine(MoveTo(inputTextField.transform.position, accumulator.transform.position, Int32.Parse(inputTextField.text)+"", true));

            setAccumulator(Int32.Parse(inputTextField.text));
            waitingOnInput = false;
            //doNextStep();
        }
    }

    public void onPlayPauseClicked()
    {
        isRunning = !isRunning;
        if (waitingOnInput)
        {
            parseInput();
        }
        else if (isRunning)
        {
            doNextStep();
        }
    }

    public void onStepClicked()
    {
        isRunning = false;
        if (waitingOnInput)
        {
            parseInput();
        }
        else
        {
            doNextStep();
        }
    }

    public void onResetClicked()
    {
        needsReset = false;
        clearAll();
        isRunning = false;
        currentCode = 0;
        clearRegisters();
    }


	public void onHelpClicked()
	{
		showHelpBox = !showHelpBox;
	}


    void clearRegisters()
    {
        for (int x = 0; x < 100; ++x)
        {
            registers[x] = 0;
        }

        foreach (GameObject reg in pRegisters) {
            if (reg != null)
            {
                reg.GetComponent<Image>().color = defaultRegister;
                reg.transform.GetChild(1).GetComponent<Text>().text = "000";
            }
        }

        onCompileClicked();

    }

	private int getAccumulator()
	{
        if (accumulator.text.Length == 0)
        {
            setAccumulator(Int32.Parse(inputTextField.text));
        }
        return Int32.Parse(accumulator.text);
	}

	private int getRegister(int reg) {
        //get value stored in register reg
        if (reg >= 100)
            return -1; //Might need a better fail value, but for now this might work.

        return registers[reg];
	}

	private int setRegister(int reg, int value) {
        //set value stored in register reg to value
        if (reg >= 100)
            return -1;

        registers[reg] = value;
        return 1;
	}
	
	private void setAccumulator(int newValue)
	{

        accumulator.text = newValue + "";
        resetInputField();
    }

    private void sendToOutput()
    {

        StartCoroutine(MoveTo(accumulator.transform.position, outputField.transform.position, getAccumulator() + "", false));

        outputField.text = getAccumulator() +"";
    }

    private void getInput()
    {
        //Change color of input field and wait for input
        accumulator.text = "";
        String input = inputTextField.text;
        if (input.Length == 0)
        {
            inputTextField.GetComponent<Image>().color = inputBoxHighlight;
            waitingOnInput = true;
            StartCoroutine(WaitForKeyDown(KeyCode.Return));
        }
        else
        {
            setAccumulator(Int32.Parse(inputTextField.text));
            //TODO : Show the transition from input to accumulator
        }
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;

        isRunning = true;
        parseInput();

    }

    private void checkForBranch(int code)
    {

        /*
        
            6XX BRA BRANCH ALWAYS

            7XX BRZ BRANCH IF ZERO

            8XX BRP BRANCH IF POSITIVE

        */

        int register = code % 100;
        int value = getAccumulator();
        int branchCode = code / 100;
        if (branchCode == 8)
        {
            if (value > 0)
            {
                //Go to the register
                branch(register);
            }
        } else if (branchCode == 7)
        {
            if (value == 0)
            {
                //Go to the register
                branch(register);
            }
        } else if (branchCode == 6)
        {
            //Go to the register
            branch(register);
        }
    }
    
    private void branch(int registerCode)
    {
        //Go to the operation 1 more than the branch code that is passed in
        if (registerCode + 1 > registers.Length)
        {
            foundError(4);
            return;
        }

        currentCode = registerCode;
        Debug.Log("Going to reg " + (registerCode + 2) + " current = " + currentCode);
    }

    private void storeOp( int code) {
		int register = code % 100;
		int Avalue = getAccumulator ();

        StartCoroutine(MoveTo(accumulator.transform.position, pRegisters[register].transform.position, getAccumulator() + "", false));

        setRegister (register, Avalue);
	}

	private void loadOp( int code) {
		int register = code % 100;
		int Rvalue = getRegister (register);

        StartCoroutine(MoveTo(pRegisters[register].transform.position, memoryDataRegister.transform.position, Rvalue + "", false));

        setAccumulator (Rvalue);
	}

	private void addOp( int code) {
		int register = code % 100;
		int Avalue = getAccumulator();
		int Rvalue = getRegister (register);

        StartCoroutine(MoveTo(pRegisters[register].transform.position, memoryDataRegister.transform.position, Rvalue+"", false));

        setAccumulator (Rvalue + Avalue);
	}

	private void subOp( int code) {
		int register = code % 100;
		int Avalue = getAccumulator();
		int Rvalue = getRegister (register);

        StartCoroutine(MoveTo(pRegisters[register].transform.position, memoryDataRegister.transform.position, Rvalue + "", false));

        setAccumulator (Avalue - Rvalue);


		//set neg flags
	}


    // GUI stuff pertaining to error messages & Help Box
    void OnGUI()
    {
        if (errorFound)
        {
            GUI.skin = errorGUISkin;
            GUI.Window(0, new Rect((Screen.width / 4), (Screen.height / 4), (Screen.width / 2), (Screen.height / 2)), ShowErrorMessagePopup, "Invalid word");

        }

		if (showHelpBox)
		{
			GUI.skin = helpGUISkin;
			GUI.Window(0, new Rect((Screen.width / 4), (Screen.height *.1f), (Screen.width / 2), (Screen.height *.75f)), ShowHelpBox, "Help");
			
		}

    }

    void ShowErrorMessagePopup(int windowID)
    {
        // You may put a label to show a message to the player
        float width = Screen.width / 2;
        float height = Screen.height / 2;

        GUI.Label(new Rect(width / 4, height / 4, width / 2, height / 2), errorStrings[errorCode]);

        // You may put a button to close the pop up too

        if (GUI.Button(new Rect(width / 2 - 35, height * 3 /4 + 10, 70, 30), "OK"))
        {
            errorFound = false;
            // After an error has been noted, we will need to either clear everything or recompile or something, this would happen here
            // Error code 3 is that the accumulator is too large. Error 4 is that we branched to an illegal place. In either case, they need to reset and fix the script.
            needsReset = errorCode == 3 || errorCode == 4;
        }

    }
    
	void ShowHelpBox(int windowID)
	{
		float width = Screen.width / 2;
		float height = Screen.height *.75f;
		Texture2D texture = new Texture2D(1, 1);
		//private Color usedRegisterBlue = new Color32(0, 207, 255, 255);
		//private Color currentOpRegCodes = new Color32(132, 249, 132, 255);
		//private Color inputBoxHighlight = new Color32(255, 252, 137, 255);
		//private Color defaultRegister = new Color32(223, 253, 255, 255);

		texture.SetPixel(0,0,defaultRegister);
		texture.Apply();
		GUI.DrawTexture (new Rect (200, 50, 20, 20), texture);
		GUI.Label (new Rect (230, 50, 500, 20), "Unused registers");

		texture.SetPixel(0,0,usedRegisterBlue);
		texture.Apply();
		GUI.DrawTexture (new Rect (200,80,20,20), texture);
		GUI.Label (new Rect (230, 80, 500, 20), "Used registers");

		texture.SetPixel(0,0,currentOpRegCodes);
		texture.Apply();
		GUI.DrawTexture (new Rect (200, 110, 20, 20), texture);
		GUI.Label (new Rect (230, 110, 500, 20), "Current operation register");

		texture.SetPixel(0,0,inputBoxHighlight);
		texture.Apply();
		GUI.DrawTexture (new Rect (200, 140, 20, 20), texture);
		GUI.Label (new Rect (230, 140, 500, 20), "Enter an input where this color appears");


		if (GUI.Button(new Rect(width / 2 - 35, height * 3 /4 -50, 70, 30), "Tutorial"))
		{
			showHelpBox = false;
			Application.LoadLevel(4);
	
		}

		if (GUI.Button(new Rect(width / 2 - 35, height * 3 /4 + 10, 70, 30), "Got it!"))
		{
			showHelpBox = false;
			
		}
		
	}


    private void foundError(int code)
    {
        errorFound = true;
        errorCode = code;
    }

    IEnumerator MoveTo(Vector3 start, Vector3 stop, String text, bool doNext)
    {

        GameObject anim = Instantiate(animationPrefab) as GameObject;
        anim.transform.SetParent(bgPanel.transform, false);
        anim.transform.GetChild(0).GetComponent<Text>().text = text;
        anim.transform.GetComponent<Image>().color = currentOpRegCodes;
        anim.transform.position = start;

        float timeEllapsed = 0;

        while (timeEllapsed < animTime)
        {
            anim.transform.position = Vector3.Lerp(start, stop, timeEllapsed / animTime);
            timeEllapsed += lerpTime;
            yield return new WaitForSeconds(lerpTime);
        }
        Destroy(anim);
        if (doNext)
            doNextStep();
    }




	


}



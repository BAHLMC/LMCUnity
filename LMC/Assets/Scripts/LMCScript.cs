using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class LMCScript : MonoBehaviour {

    // Private booleans for switching between modes
    private bool isRunning;
    private bool inEditMode;

    //Variables needed for parsing and running the op codes
    private int currentCode;
    private string[] opCodes;
    private bool isNegative;

    //Variables used for the registers
    private int[] registers;
    private GameObject[] pRegisters;

    //Color presets
    private Color yellow = new Color32(245, 255, 69, 255);
    private Color green = new Color32(0, 255, 0, 255);
    private Color blue = new Color32(69, 247, 255, 255);
    private int prevCode = 0;

    //Interactive variables set in Unity
    public Text parsedTextBox;
    public InputField scriptInput;
    public InputField outputField;
    public InputField accumulator;
    public InputField programCounter;
    public InputField inputTextField;
    public GameObject registerPanel;
    public GameObject registerPrefab;
    public float autoRunDelay = 1f;

    /*

    901 INP INPUT

    902 OUT OUTPUT

    DAT DATA

    */

    // Use this for initialization
    void Start () {

        isRunning = false;
        inEditMode = false;

        registers = new int[100];
        pRegisters = new GameObject[100];
        clearAll();

        //Test
        parsedTextBox.text = "The parsed text from the script will go here";

        scriptInput.text = "INP\nSTA FIRST\nINP\nSTA SECOND\nINP\nADD FIRST\nADD SECOND\nSUB FIRST\nOUT\nHLT\nFIRST DAT\nSECOND DAT";

        opCodes = new string[0];
        currentCode = -1;

        float regWidth = registerPanel.GetComponent<RectTransform>().rect.width / 10;
        float regHeight = registerPanel.GetComponent<RectTransform>().rect.height / 10;

        for (int dx = 0; dx < 10; ++dx){
            for (int dy = 0; dy < 10; ++dy)
            {
               GameObject reg = Instantiate(registerPrefab) as GameObject;
                //reg.transform.parent = registerPanel.transform;
                reg.transform.SetParent(registerPanel.transform, false);
                reg.transform.localPosition = new Vector3(regWidth*dy - registerPanel.GetComponent<RectTransform>().rect.height/2, -regHeight*dx + registerPanel.GetComponent<RectTransform>().rect.width/2 - regHeight, 0);
                pRegisters[10 * dx + dy] = reg;
                reg.transform.GetChild(0).GetComponent<Text>().text = 10 * dx + dy + "";

            }
        }
    }

    // Update is called once per frame
    void Update () {
        scriptInput.enabled = inEditMode;
        for (int x = 0; x < 100; ++x)
        {
            if (registers[x] != 0)
                pRegisters[x].transform.GetChild(1).GetComponent<Text>().text = registers[x] + "";
            else
                pRegisters[x].transform.GetChild(1).GetComponent<Text>().text = "000";
        }
        programCounter.text = currentCode + "";

	}

    void clearAll()
    {
        outputField.text = "";
        accumulator.text = "";
        inputTextField.text = "";
    }

    void doNextStep()
    {
        if (currentCode > opCodes.Length || currentCode < 0)
        {
            isRunning = false;
            return;
        }

        //update register colors
        pRegisters[prevCode].GetComponent<Image>().color = yellow;
        pRegisters[currentCode].GetComponent<Image>().color = blue;
        prevCode = currentCode;

        int code = Int32.Parse(opCodes[currentCode]);
        
		if (code == 902) {
			sendToOutput ();
		} else if (code == 901) {
			getInput ();
		} else if (code >= 600) {
			checkForBranch (code);
		} else if (code >= 500) {
			loadOp (code);
		} else if (code >= 300) {
			storeOp(code);
		} else if (code >= 200) {
			subOp (code);
		} else if (code >= 100) {
			addOp (code);
		} else if (code == 000) {
            //halt
            isRunning = false;
            return;
		}

        currentCode++;
        if (isRunning)
        {
            StartCoroutine(Pause());
            doNextStep();
        }
    }

    public void onEditClicked()
    {
        inEditMode = true;
    }

    public void onSaveClicked()
    {
        inEditMode = false;
        String curText = scriptInput.text;
        string[] input = curText.Split('\n');
        string[] result = LoadScript.callStartScanTest(input);
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

    public void onPlayPauseClicked()
    {
        isRunning = !isRunning;
        if (isRunning)
            doNextStep();
    }

    public void onRunClicked()
    {
        isRunning = true;
        doNextStep();
    }

    public void onResetClicked()
    {
        clearAll();
        isRunning = false;
        currentCode = 0;
    }

    public void onStepClicked()
    {
        isRunning = false;
        doNextStep();
    }


	private int getAccumulator()
	{
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
	}

    private void sendToOutput()
    {
        outputField.text = getAccumulator() +"";
    }

    private void getInput()
    {
        //Change color of input field and wait for input
        String input = inputTextField.text;
        if (input.Length == 0)
            input = "5";
        setAccumulator(Int32.Parse(input));
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
                branch(branchCode);
            }
        } else if (branchCode == 7)
        {
            if (value == 0)
            {
                //Go to the register
                branch(branchCode);
            }
        } else if (branchCode == 6)
        {
            //Go to the register
            branch(branchCode);
        }
    }
    
    private void branch(int branchCode)
    {
        //Go to the operation 1 more than the branch code that is passed in
    }
	

	private void loadOp( int code) {
		int register = code % 100;
		int Avalue = getAccumulator ();
		setRegister (register, Avalue);
	}

	private void storeOp( int code) {
		int register = code % 100;
		int Rvalue = getRegister (register);
		setAccumulator (Rvalue);
	}

	private void addOp( int code) {
		int register = code % 100;
		int Avalue = getAccumulator();
		int Rvalue = getRegister (register);
		setAccumulator (Rvalue + Avalue);
	}

	private void subOp( int code) {
		int register = code % 100;
		int Avalue = getAccumulator();
		int Rvalue = getRegister (register);
		setAccumulator (Avalue - Rvalue);
		//set neg flags
	}

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(autoRunDelay);
    }

}

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

    //Interactive variables set in Unity
    public Text parsedTextBox;
    public InputField scriptInput;
    public InputField outputField;
    public InputField accumulator;
    public InputField inputTextField;

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
        clearAll();

        //Test
        parsedTextBox.text = "The parsed text from the script will go here";

        scriptInput.text = "INP\nSTA FIRST\nINP\nSTA SECOND\nINP\nADD FIRST\nADD SECOND\nSUB FIRST\nOUT\nHLT\nFIRST DAT\nSECOND DAT";

        opCodes = new string[0];
        currentCode = -1;
    }

    // Update is called once per frame
    void Update () {
        scriptInput.enabled = inEditMode;
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
            doNextStep();
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

}

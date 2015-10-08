using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LMCScript : MonoBehaviour {

    // Private booleans for switching between modes
    private bool isRunning;
    private bool inEditMode;

    //Variables needed for parsing and running the op codes
    private Node currentCode;

    //Interactive variables set in Unity
    public Text parsedTextBox;
    public InputField scriptInput;
    public InputField outputField;
    public Text accumulator;
    public InputField inputTextField;

    /*

    901 INP INPUT

    902 OUT OUTPUT

    DAT DATA

    */

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
        int code = currentCode.code();
        if (code == 902)
        {
            sendToOutput();
        } else if (code == 901)
        {
            getInput();
        } else if (code >= 600)
        {
            checkForBranch(code);
        }

        currentCode = currentCode.next();
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

    private void sendToOutput()
    {
        //Get value from accumulator
        //send value to output
    }

    private void getInput()
    {
        //Change color of input field and wait for input

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

    private int getAccumulator()
    {
        return Int32.Parse(accumulator.text);
    }

    private void setAccumulator(int newValue)
    {
        accumulator.text = newValue + "";
    }

    private class Node
    {
        int opCode;
        Node prevNode;
        Node nextNode;

        public int code ()
        {
            return opCode;
        }

        public Node next()
        {
            return nextNode;
        }
    }

}

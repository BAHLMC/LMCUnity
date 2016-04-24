using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

class File
{
    public string filename;
    public string contents;
    File(string _filename, string _contents){
        filename = _filename;
        contents = _contents;
    }
}
public class LoadScript : MonoBehaviour
{
    string[] filenamesArr;
    int currFile = 0;

    /*
    
        //This is the fake iput for dummy testing
        INP\nSTA FIRST\nINP\nSTA SECOND\nINP\nADD FIRST\nADD SECOND\nSUB FIRST\nOUT\nHLT\nFIRST DAT\nSECOND DAT
        
        INP
        STA FIRST
        INP
        STA SECOND
        INP
        ADD FIRST
        ADD SECOND
        SUB FIRST
        OUT
        HLT 
        FIRST DAT 
        SECOND DAT

        */

    public Text saveScriptInput;
    public InputField filenameText;
    public Text filenames;
    public Text selected;
    void Start(){
        filenamesArr = loadFilenames();
        displayFilenames(filenamesArr);
        loadTexts();
        processLoad();

	}
    void displayFilenames(string[] names)
    {
        string str = "";
        foreach(string s in names){
            str += s + "\n";
            Debug.Log("displaying " + s);
        }
        filenames.text = str;
    }
    void loadTexts()
    {
        string scriptText = PlayerPrefs.GetString("currentScriptText", "NONE");
        string filename = PlayerPrefs.GetString("filename", "404");
        if (scriptText != "NONE")
        {
            saveScriptInput.text = scriptText;
        }
        else
        {
            saveScriptInput.text = "";
        }
        if (filename != "404")
        {
            filenameText.text = filename;
        }
        else
        {
            filenameText.text = "";
        }
    }

    public void saveFile()
    {
        string saveto = "file" + ((filenamesArr.Length <= 9) ? "0" + filenamesArr.Length.ToString() : filenamesArr.Length.ToString());
        if (PlayerPrefs.GetString(saveto, "") == "")
        {
            PlayerPrefs.SetString(saveto, filenameText.text);
        }
        PlayerPrefs.SetString("contents" + filenameText.text, saveScriptInput.text);
        Debug.Log("saving " + filenameText.text + " to " + saveto);
        PlayerPrefs.SetString("contents" + filenameText.text, saveScriptInput.text);
        filenamesArr = loadFilenames();
        displayFilenames(filenamesArr);
        loadTexts();
    }
    public void loadLMC()
    {
        Application.LoadLevel(1);
    }
    public void clearSaves()
    {
        for(int i = 0; i < filenamesArr.Length; i++)
        {
            string command = "file" + ((i <= 9) ? "0" + i.ToString() : i.ToString());
            PlayerPrefs.SetString(command, "");
        }
    }
    public void modifyCurrentFilename()
    {
        if (filenameText.text != "")
        {
            PlayerPrefs.SetString("filename", filenameText.text);
        }
    }
    public void GoToMainMenu ()
	{
		Application.LoadLevel (3);
	}

    public void upButton()
    {
        currFile--;
        if(currFile < 0)
        {
            currFile = filenamesArr.Length - 1;
        }
        processLoad();
    }
    public void loadCurrent()
    {
        string savedText = PlayerPrefs.GetString("contents" + filenamesArr[currFile], "404");
        if(savedText == "404")
        {
            Debug.Log("could not find file");
        }
        else
        {
            PlayerPrefs.SetString("currentScriptText", savedText);
            PlayerPrefs.SetString("filename", filenamesArr[currFile]);
            loadTexts();
        }
    }
    public void downButton()
    {
        currFile++;
        if(currFile >= filenamesArr.Length)
        {
            currFile = 0;
        }
        processLoad();
    }

    public static string[] callStartScanTest (string[] toTest)
	{
        return StartScan(toTest);
	}
    void processLoad()
    {
        string selStr = "";
        for(int i = 0; i < filenamesArr.Length; i++)
        {
            if(i == currFile)
            {
                selStr += filenamesArr[i];
                Debug.Log("setting selected to " + filenamesArr[i] + " i = " + i);
            }
            else
            {
                selStr += "\n";
            }
        }
        selected.text = selStr;
    }

	static string convertRegToString (int reg)
	{
		try{
			if (reg < 10) {
				return "0" + reg.ToString ();
			} else {
				return reg.ToString ();
			}
		} catch (KeyNotFoundException e){
			Debug.Log ("Key not found");
			return "-1";
		}
	}

    string[] loadFilenames()
    {
        List<string> strs = new List<string>();
        string str = PlayerPrefs.GetString("file00", "");
        int counter = 1;
        while(str != "")
        {
            strs.Add(str);
            string command = "file" + (counter <= 9 ? "0" + counter.ToString() : counter.ToString());
            str = PlayerPrefs.GetString(command, "");
            Debug.Log("loader on " + command + " found " + str);
            counter++;
        }
        return strs.ToArray();
    }

	static string[] StartScan (string[] al)
	{
		var dats = new Dictionary<string,int> ();
		for (int i = 0; i <al.Length; i++) {
			if (al [i].Contains ("DAT")) {
				string[] arr = al [i].Split (' ');
				if(arr[0].Contains("DAT")){
					dats [arr [1]] = i;
				}else if(arr[1].Contains("DAT")){
					dats [arr [0]] = i;
				}
			}
		}
		string[] ret = new string[al.Length];
		for (int i = 0; i < al.Length; i++) {
			try{
				string s = al [i];
				string[] arr = s.Split (' ');
				if (arr [0] == "HLT")
					ret [i] = ("000");
				else if (arr [0] == "INP")
					ret [i] = ("901");
				else if (arr [0] == "OUT")
					ret [i] = ("902");
				else if (arr [0] == "ADD") {
					ret [i] = "1" + convertRegToString (dats [arr [1]]);
				} else if (arr [0] == "SUB") {
					ret [i] = "2" + convertRegToString (dats [arr [1]]);
				} else if (arr [0] == "STA") {
					ret [i] = "3" + convertRegToString (dats [arr [1]]);
				} else if (arr [0] == "LDA") {
					ret [i] = "5" + convertRegToString (dats [arr [1]]);
				} else if (arr [0] == "BRA") {
					ret [i] = "6" + convertRegToString (dats [arr [1]]);
				} else if (arr [0] == "BRZ") {
					ret [i] = "7" + convertRegToString (dats [arr [1]]);
				} else if (arr [0] == "BRP") {
					ret [i] = "8" + convertRegToString (dats [arr [1]]);
				}else if(arr[0]  == ""){

				}
				else if (arr.Length <= 1 || arr[1] != "DAT")
            	{
	                //If it is none of the above commands and not related to a DAT register, we will return this erro code
                	ret[i] = "-1";
            	}
			}catch(KeyNotFoundException e){
				ret[i] = "-1";
			}

		}
		return ret;
	}
}

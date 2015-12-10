using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoadScript : MonoBehaviour
{

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
    public void GoToMainMenu ()
	{
		Application.LoadLevel (3);
	}

	public static string[] callStartScanTest (string[] toTest)
	{
        return StartScan(toTest);
	}

	static string convertRegToString (int reg)
	{
		if (reg < 10) {
			return "0" + reg.ToString ();
		} else {
			return reg.ToString ();
		}
	}

	static string[] StartScan (string[] al)
	{
		var dats = new Dictionary<string,int> ();
		for (int i = 0; i <al.Length; i++) {
			if (al [i].Contains ("DAT")) {
				string[] arr = al [i].Split (' ');
				dats [arr [0]] = i;
			}
		}
		string[] ret = new string[al.Length];
		for (int i = 0; i < al.Length; i++) {
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
			} else if (arr.Length <= 1 || arr[1] != "DAT")
            {
                //If it is none of the above commands and not related to a DAT register, we will return this erro code
                ret[i] = "-1";
            }

		}
		return ret;
	}
}

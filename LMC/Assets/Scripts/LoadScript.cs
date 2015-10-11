using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadScript : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void GoToMainMenu ()
	{
		Application.LoadLevel (3);
	}

	public void callStartScanTest ()
	{
		string[] toTest = new string[12] {
			"INP","STA FIRST","INP","STA SECOND","INP",
			"ADD FIRST","ADD SECOND","SUB FIRST","OUT","HLT","FIRST DAT","SECOND DAT"
		};
		string[] ret = StartScan (toTest);
		for (int i = 0; i < ret.Length; i++) {
			Debug.Log (ret [i]);
		}
	}

	string convertRegToString (int reg)
	{
		if (reg < 10) {
			return "0" + reg.ToString ();
		} else {
			return reg.ToString ();
		}
	}

	string[] StartScan (string[] al)
	{
		var dats = new Dictionary<string,int> ();
		for (int i = 0; i <al.Length; i++) {
			if (al [i].Contains ("DAT")) {
				string[] arr = al [i].Split (' ');
				dats [arr [0]] = i;
			}
		}
		string[] ret = new string[al.Length];
		List<string> needed = new List<string> ();
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
			}

		}
		return ret;
	}
}

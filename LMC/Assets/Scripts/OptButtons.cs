using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class OptButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void saveFile()
    {
        string scriptText = PlayerPrefs.GetString("currentScriptText", "404");
        string path = PlayerPrefs.GetString("filename", "404");
        string temp = EditorUtility.SaveFilePanel(
            "1", path.Substring(path.LastIndexOf("/"),path.Length - path.LastIndexOf("/")), "untitled","txt");
        if(path.Length > 0)
        {
            File.WriteAllText(temp,scriptText);
        }
    }

	public void SelectFile () {
        string path = EditorUtility.OpenFilePanel(
            "Test1",
            "Test2",
            "txt"
            );
        if (path.Length != 0)
        {
            WWW www = new WWW("file:///" + path);
            PlayerPrefs.SetString("currentScriptText", File.ReadAllText(path));
            PlayerPrefs.SetString("filename", path);
            Application.LoadLevel(1);
        }
	}

    public void GoToMainMenu()
    {
        Application.LoadLevel(0);
    }

    public void QualityLow()
    {
        
    }

    public void QualityMed()
    {
        
    }

    public void QualityHigh()
    {
        
    }
}

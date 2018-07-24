using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Text;

public class InfoButton : MonoBehaviour {
    public string playerName;

    public InterfaceManager interfaceManager;

    char tab;

    [SerializeField]
    private Button infoButton;

    
	// Use this for initialization
	void Start ()
    {
       
        infoButton.onClick.AddListener(delegate
        {
            ShowInfo();
        });
    }
	
    void ShowInfo()
    {
        tab = Convert.ToChar("\t");
        string[] historicLines;
        string output = "";

        historicLines = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - Historic.txt", Encoding.UTF8);
        Debug.Log("Historic Size: " + historicLines.Length);

        for( int n = 1; n<historicLines.Length; n++)
        {   

            string[] lineColumns = historicLines[n].Split(tab);
            output = output + lineColumns[1] + " | " + lineColumns[4] + "|" + lineColumns[9] + "|" + lineColumns[12] + "|" + lineColumns[15] + " |" + Environment.NewLine;
        }
        
        interfaceManager.infoBox.text = output;
        Debug.Log(playerName);
        
    }
}

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
        string output1 = "";
        string output2 = "";
        string output3 = "";
        string output4 = "";
        string output5 = "";

        historicLines = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - Historic.txt", Encoding.UTF8);
        Debug.Log("Historic Size: " + historicLines.Length);

        for( int n = 1; n<historicLines.Length; n++)
        {   

            string[] lineColumns = historicLines[n].Split(tab);
            output1 = output1 + lineColumns[1] + Environment.NewLine;
            output2 = output2 + lineColumns[4] + Environment.NewLine;
            output3 = output3 + lineColumns[9] + Environment.NewLine;
            output4 = output4 + lineColumns[12] + Environment.NewLine;
            output5 = output5 + lineColumns[15] + Environment.NewLine;
            interfaceManager.infoBox.text = "";
        }
        interfaceManager.infoBoxColumn1.text = output1;
        interfaceManager.infoBoxColumn2.text = output2;
        interfaceManager.infoBoxColumn3.text = output3;
        interfaceManager.infoBoxColumn4.text = output4;
        interfaceManager.infoBoxColumn5.text = output5;

        Debug.Log(playerName);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Text;

public class InterfaceManager : MonoBehaviour
{

    public GameObject templateToggle;
    public GameObject toggleParent;

    public InterfaceManager interfaceManager;

    public InputField infoBox;

    private string[] playersText;

    // Use this for initialization
    void Start () {
        playersText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
        Debug.Log(playersText.Length);
      
        for (int n = 0; n< playersText.Length; n++)
        {
            
            GameObject toggleAux;
            Text textAux;
            InfoButton infoButtonAux;
            Button buttonAux;

            toggleAux = Instantiate(templateToggle, toggleParent.transform);
            toggleAux.name =  playersText[n]  ;

            textAux = toggleAux.GetComponentInChildren<Text>();
            textAux.text =  playersText[n];

            buttonAux = toggleAux.GetComponentInChildren<Button>();
            buttonAux.name = "Button_" + playersText[n];

            infoButtonAux = toggleAux.GetComponent<InfoButton>();
            infoButtonAux.playerName = playersText[n];
            infoButtonAux.interfaceManager = this;
     
        }

 
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

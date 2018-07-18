using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

public class InterfaceManager : Analyze {

    public GameObject templateToggle;
    public GameObject toggleParent;

  /*  public string playerName;
    private string[] movementsText;*/
  

    // Use this for initialization
    void Start () {

      
        for (int n = 0; n< 6; n++)
        {
            
            GameObject toggleAux;
            Text textAux;
                     
            toggleAux = Instantiate(templateToggle, toggleParent.transform);
            toggleAux.name = "Pacient_" + (n + 1).ToString() ;

            textAux = toggleAux.GetComponentInChildren<Text>();
            textAux.text = "Pacient_" + (n + 1).ToString();
        }

       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

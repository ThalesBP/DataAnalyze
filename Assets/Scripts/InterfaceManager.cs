using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {

    public GameObject templateToggle;
    public GameObject toggleParent;
  //  public GameObject templateButton;
//    public GameObject buttonParent;

	// Use this for initialization
	void Start () {

         
        for (int n = 0; n<20; n++)
        {
            GameObject buttonAux;
            GameObject toggleAux;
            Text textAux;
                     
            toggleAux = Instantiate(templateToggle, toggleParent.transform);
            toggleAux.name = "Pacient_" + (n + 1).ToString();

         //   buttonAux = Instantiate(templateButton, buttonParent.transform);

            textAux = toggleAux.GetComponentInChildren<Text>();
            textAux.text = "Pacient_" + (n + 1).ToString();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

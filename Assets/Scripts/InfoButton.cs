using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour {
    public string playerName;

    public InterfaceManager interfaceManager;

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
        Debug.Log(playerName);
        interfaceManager.infoBox.text = playerName;
    }
}

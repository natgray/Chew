using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pericam : MonoBehaviour {
    
    private bool periscoped;
    public Camera main, pericam;
    private int rabbitState = 0;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.T) && rabbitState == 0)
        {
            // periscope
            rabbitState = 1; //switch state
            periscope();

        }
        else if (Input.GetKeyDown(KeyCode.T) && rabbitState == 1)
        {
            rabbitState = 0; //switch state
            unPeriscope();
        }

    }
    void periscope()
    {
        // Periscope
        
        main.enabled = false;
        pericam.enabled = true;
    }

    void unPeriscope()
    {
        main.enabled = true;
        pericam.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void OnClick()
    {
        Time.timeScale = 1.0f;
        PauseMenuManager.pauseFlag = false;
    }
}

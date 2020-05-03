using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public static SoundManager soundmanager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SoftClick()
    {
        soundmanager.SoftClickUI();
    }

    public void HardClick()
    {
        soundmanager.HardClickUI();
    }
}

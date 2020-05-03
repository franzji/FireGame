using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> audioclips;
    private AudioSource audio;

    // Use this for initialization
    void Start ()
    {
        audio = GetComponent<AudioSource>();

        ButtonSounds.soundmanager = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SoftClickUI()
    {
        audio.clip = audioclips[0];
        audio.Play();
    }

    public void HardClickUI()
    {
        audio.clip = audioclips[1];
        audio.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MasterVolume : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    public void setMixerVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
}

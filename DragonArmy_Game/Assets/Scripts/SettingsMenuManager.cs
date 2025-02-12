using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown graphicDropdown;
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer mainAudioMixer;


    public void ChangeGraphicQuality()
    {
        QualitySettings.SetQualityLevel(graphicDropdown.value);
    }

    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }
    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("MusicVol", musicVol.value);
    }
    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFXVol", sfxVol.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

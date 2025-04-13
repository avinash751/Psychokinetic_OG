using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;



[System.Serializable]
public class VolumeSettingsData : SettingSO
{
    public MMSoundManager.MMSoundManagerTracks volumeType;
    public float minVolume = 0;
    public float maxVolume = 1;
    public float currentVolume = 1;
    [SerializeField] MMSoundManagerSettingsSO soundSettingsSO;

    
    public void SetTrackVolume(float sliderValue)
    {
        currentVolume = sliderValue;
        soundSettingsSO.SetTrackVolume(volumeType, currentVolume);
        PlayerPrefs.SetFloat(volumeType.ToString(), currentVolume);
    }

    public float GetTrackVolume()
    {
        if (PlayerPrefs.HasKey(volumeType.ToString()))
        {
            currentVolume = PlayerPrefs.GetFloat(volumeType.ToString(), currentVolume);
            return currentVolume;
        }
        return currentVolume = 1;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class BrightnessSettingData : SettingSO
{
    [SerializeField] public VolumeProfile brightnessVolume;
    [SerializeField] ColorAdjustments brightnessComponent;
    [SerializeField] public float lowestBrightness = -10;
    [SerializeField] public float highestBrightness = 10;

    public BrightnessSettingData()
    {
        settingName = GetType().Name;
    }


    public void GetBrightness(out float brightnessValue)
    {
        brightnessVolume.TryGet(out brightnessComponent);
        if (PlayerPrefs.HasKey(settingName))
        {
            brightnessValue = PlayerPrefs.GetFloat(settingName);
        }
        else
        {
            brightnessValue = 0;
        }
    }

    public void SetBrightness(float sliderValue)
    {
        brightnessVolume.TryGet(out brightnessComponent);
        if (brightnessComponent != null)
        {
            float actualVolumeBrightnessValue = Mathf.Lerp(lowestBrightness, highestBrightness, sliderValue);
            brightnessComponent.postExposure.value = actualVolumeBrightnessValue;

            PlayerPrefs.SetFloat(settingName, sliderValue);
        }
    }

}

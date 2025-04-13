using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class BrightnessSetting : Setting
{
    [SerializeField] Slider brightnessSlider;
    [SerializeField][HideInInspector] BrightnessSettingData brightnessSettingData;

    public BrightnessSetting(BrightnessSettingData dataHolder)
    {
        name = GetType().Name;
        brightnessSettingData = dataHolder;
    }
    public override void Initialize()
    {
        brightnessSettingData.GetBrightness(out float currentBrightness);
        brightnessSlider.value = currentBrightness;
        brightnessSettingData.SetBrightness(currentBrightness);
        ConnectUiToLogic();
        
    }

    protected override void ConnectUiToLogic()
    {
        UnityAction<float> ChangeBrightnessDelegate = value =>
        {
            brightnessSettingData.SetBrightness(value);
        };
        brightnessSlider.onValueChanged.AddListener(ChangeBrightnessDelegate);       
    }
}

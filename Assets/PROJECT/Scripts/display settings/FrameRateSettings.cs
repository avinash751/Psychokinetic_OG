using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[Serializable]
public class FrameRateSettings : Setting
{
    [SerializeField] ListSelect frameRateSelection;
    [SerializeField] FrameRateSettingsData frameRateSettingsData;
    public FrameRateSettings(FrameRateSettingsData dataHolder)
    {
        name = GetType().Name;
        frameRateSettingsData = dataHolder;
    }

    public override void Initialize()
    {
        foreach (var frameRate in frameRateSettingsData.frameRates)
        {
            frameRateSelection.OptionList.Add(frameRate.ToString());
        }
        frameRateSettingsData.GetCurrentTargetFrameRate(out int currentFrameRate);
        frameRateSettingsData.SetTargetFrameRate(currentFrameRate);

        frameRateSelection.extraText = " FPS";
        int index = Array.IndexOf(frameRateSettingsData.frameRates, currentFrameRate);
        frameRateSelection.UpdateIndexAndText(index);
        ConnectUiToLogic();        
    }

    protected override void ConnectUiToLogic()
    {
        frameRateSelection.OnPress.AddListener(() =>
        {
            int frameRate = frameRateSettingsData.frameRates[frameRateSelection.CurrentIndex];
            frameRateSettingsData.SetTargetFrameRate(frameRate);
        });
       
    }
}
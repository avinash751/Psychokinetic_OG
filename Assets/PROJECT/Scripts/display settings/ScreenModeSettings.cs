using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ScreenModeSettings : Setting
{
    [SerializeField] ListSelect screenModeSelection;
    [HideInInspector][SerializeField] ScreenModeSettingData screenModeSettingData;
    public ScreenModeSettings(ScreenModeSettingData dataHolder)
    {
        name = GetType().Name;
        screenModeSettingData = dataHolder;
    }

    protected override void ConnectUiToLogic()
    {
        screenModeSelection.OnPress.AddListener(() =>
        {
            FullScreenMode mode = screenModeSettingData.screenModes[screenModeSelection.CurrentIndex];
            screenModeSettingData.SetScreenMode(mode);
        });
    }

    public override void Initialize()
    {

        foreach (var option in screenModeSettingData.screenModes)
        {
            screenModeSelection.OptionList.Add(option.ToString());
        }
        screenModeSettingData.GetScreenMode(out FullScreenMode mode);
        int index = Array.IndexOf(screenModeSettingData.screenModes, mode);
        screenModeSelection.UpdateIndexAndText(index);
        ConnectUiToLogic();
    }
}



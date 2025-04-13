using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using TMPro;

[Serializable]
public class VSyncSettings : Setting
{
    [SerializeField] VSyncSettingsData vSyncSettingsData;
    [SerializeField] ListSelect vSyncSelection;
      
    public VSyncSettings(VSyncSettingsData dataHolder)
    {
        name = GetType().Name;
        vSyncSettingsData = dataHolder;
    }
    public override void Initialize()
    {
        foreach (var vSyncOption in vSyncSettingsData.vSyncOptions)
        {
            vSyncSelection.OptionList.Add(vSyncOption);
        }
        vSyncSettingsData.GetCurrentVSync(out int currentVSync);
        vSyncSelection.UpdateIndexAndText(currentVSync);
        ConnectUiToLogic();      
    }

    protected override void ConnectUiToLogic()
    {
        vSyncSelection.OnPress.AddListener(() =>
        {
            int vSync = vSyncSelection.CurrentIndex;
            vSyncSettingsData.SetVSync(vSync);
        });    
    }
}

using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

[Serializable]
public class ResolutionSettings : Setting
{
    [SerializeField] ResolutionSettingsData resolutionSettingsData;
    [SerializeField] ListSelect resolutionSelection;
    public ResolutionSettings( ResolutionSettingsData dataHolder)
    {
        name = GetType().Name;
        resolutionSettingsData = dataHolder;
    }
    public override void Initialize()
    {
        resolutionSelection.OptionList = new List<string>();
        for (int i = 0; i < resolutionSettingsData.resolutions.Length; i++)
        {

            float currentWidth = resolutionSettingsData.resolutions[i].width;
            float currentHeight = resolutionSettingsData.resolutions[i].height;

            if (i > 0)
            {
                float previousWidth = resolutionSettingsData.resolutions[i - 1].width;
                float previousHeight = resolutionSettingsData.resolutions[i - 1].height;
                if (currentWidth == previousWidth && currentHeight == previousHeight) { continue; }
            }
           resolutionSelection.OptionList.Add(currentWidth + " x " + currentHeight);
        }
        resolutionSettingsData.GetCurrentResolution(out Resolution resolution);
        int index = resolutionSelection.OptionList.IndexOf(resolution.width + " x " + resolution.height);
        resolutionSelection.UpdateIndexAndText(index);
        ConnectUiToLogic();
    }

    protected override void ConnectUiToLogic()
    {
       resolutionSelection.OnPress.AddListener(() =>
       {
           float Index = resolutionSelection.CurrentIndex!=0? resolutionSelection.CurrentIndex:0;
           Resolution resolution = resolutionSettingsData.resolutions[resolutionSelection.CurrentIndex +21];
           resolutionSettingsData.SetResolution(resolution.width,resolution.height);
       });
    }
}
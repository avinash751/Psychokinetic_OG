using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public  class ResolutionSettingsData: SettingSO
{
    public Resolution[] resolutions => Screen.resolutions;

    public ResolutionSettingsData()
    {
        settingName = GetType().Name;
    }

    public void GetCurrentResolution(out Resolution resolution)
    {
        resolution = Screen.currentResolution;
    }   

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
    }
}


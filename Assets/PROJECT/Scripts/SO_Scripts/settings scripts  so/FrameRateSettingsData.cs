using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FrameRateSettingsData : SettingSO
{
    public int[] frameRates = new int[] { 30, 60, 90, 120, 240 };

    public FrameRateSettingsData()
    {
        settingName = GetType().Name;
    }

    public void GetCurrentTargetFrameRate(out int frameRate)
    {
        if (!PlayerPrefs.HasKey("FrameRate"))
        {
            frameRate =  Application.targetFrameRate;
            PlayerPrefs.SetInt("FrameRate", frameRate);
        }
        else
        { frameRate = 30; }
    }

    public void SetTargetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
        PlayerPrefs.SetInt("FrameRate", frameRate);
    } 
}

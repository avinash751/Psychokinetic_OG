using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class VSyncSettingsData : SettingSO
{
    public string[] vSyncOptions = new string[] { "Off", "Every V Blank","Every Second V Blank" };
    public VSyncSettingsData()
    {
        settingName = GetType().Name;
    }
    public void GetCurrentVSync(out int currentVSync)
    {
        if(PlayerPrefs.HasKey(settingName))
        {
            currentVSync = PlayerPrefs.GetInt(settingName);
        }
        else
        {
            currentVSync = QualitySettings.vSyncCount;
        }
    }
    public void SetVSync(int vSync)
    {
        QualitySettings.vSyncCount = vSync;
        PlayerPrefs.SetInt(settingName, vSync);
    }
}

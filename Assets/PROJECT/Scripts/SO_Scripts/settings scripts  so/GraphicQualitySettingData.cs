using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphicQualitySettingData : SettingSO
{
   public string[] qualityLevels => QualitySettings.names;

    public GraphicQualitySettingData()
    {
        settingName = GetType().Name;
    }

    public void GetCurrentQualityLevel(out int qualityLevel)
    {
        if(PlayerPrefs.HasKey(settingName))
        {
            qualityLevel = PlayerPrefs.GetInt(settingName);
        }
        else
        qualityLevel = QualitySettings.GetQualityLevel();
    }

    public void SetQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        PlayerPrefs.SetInt(settingName, qualityLevel);
    }   
}

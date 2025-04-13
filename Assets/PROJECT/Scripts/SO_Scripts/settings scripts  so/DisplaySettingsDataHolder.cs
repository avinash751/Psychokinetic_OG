using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "DisplaySettingsData", menuName = "Settings/DisplaySettingsData", order = 1)]
public class DisplaySettingsDataHolder : ScriptableObject
{
    [ContextMenuItem("Add ScreenMode Settings", nameof(AddScreenModeSettings))]
    [ContextMenuItem("Add Resolution Settings", nameof(AddResolutionSettings))]
    [ContextMenuItem("Add FrameRate Settings", nameof(AddFrameRateSettings))]
    [ContextMenuItem("Add VSync Settings", nameof(AddVSyncSettings))]
    [ContextMenuItem("Add Brightness Settings", nameof(AddBrightnessSetting))]
    [ContextMenuItem("Add Graphic Quality Settings", nameof(AddGraphicQualitySettings))]
    [SerializeReference] public List<SettingSO> SettingsSOData = new List<SettingSO>();
    [SerializeField] DisplaySettingsConnector settingsConnector;


    [ContextMenu("Add ScreenMode Settings")]
    public void AddScreenModeSettings() => AddSettingData<ScreenModeSettingData, ScreenModeSettings>();

    [ContextMenu("Add Resolution Settings")]
    public void AddResolutionSettings()=> AddSettingData<ResolutionSettingsData, ResolutionSettings>();

    [ContextMenu("Add FrameRate Settings")]
    public void AddFrameRateSettings() => AddSettingData<FrameRateSettingsData, FrameRateSettings>();

    [ContextMenu("Add VSync Settings")]
    public void AddVSyncSettings() => AddSettingData<VSyncSettingsData, VSyncSettings>();

    [ContextMenu("Add Brightness Settings")]
    public void AddBrightnessSetting() => AddSettingData<BrightnessSettingData, BrightnessSetting>();

    [ContextMenu("Add Graphic Quality Settings")]
    public void AddGraphicQualitySettings() => AddSettingData<GraphicQualitySettingData, GraphicQualitySetting>();

    void AddSettingData<SettingSOType,SettingType>() where SettingSOType : SettingSO  where SettingType : Setting
    {
        if(!DisplayConnectorInitialized()) return;
        if(CheckIfSettingExists<SettingType>()) return;
        SettingSOType settingSoData = Activator.CreateInstance<SettingSOType>();
        if (settingSoData == null) return;
        SettingsSOData.Add(settingSoData);
        Type settingType = typeof(SettingType);
        ConstructorInfo settingConstructor = settingType.GetConstructor(new Type[] { typeof(SettingSOType) });
        SettingType setting = (SettingType)settingConstructor.Invoke(new object[] { settingSoData });
        settingsConnector.appliedDisplaySettings.Add(setting);
    }
    bool CheckIfSettingExists<SettingType>() where SettingType : Setting
    {
        if(!DisplayConnectorInitialized()) return false;
        foreach (var setting in settingsConnector.appliedDisplaySettings)
        {
            if (setting.GetType() == typeof(Setting)) return true;
        }
        return false;
    }
    bool DisplayConnectorInitialized()
    {
        if (settingsConnector == null)
        {
            settingsConnector = FindObjectOfType<DisplaySettingsConnector>();
            if (settingsConnector == null) return false;
        }
        return true;
    }
}

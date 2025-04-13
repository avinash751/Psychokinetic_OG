using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;

public class DisplaySettingsConnector : MonoBehaviour
{
    public DisplaySettingsDataHolder settingData;

    [ContextMenuItem("Add ScreenMode Settings", nameof(AddScreenModeSettings))]
    [ContextMenuItem("Add Resolution Settings", nameof(AddResolutionSettings))]
    [ContextMenuItem("Add FrameRate Settings", nameof(AddFrameRateSettings))]
    [ContextMenuItem("Add VSync Settings", nameof(AddVSyncSettings))]
    [ContextMenuItem("Add Brightness Settings", nameof(AddBrightnessSetting))]
    [ContextMenuItem("Add Graphic Quality Settings", nameof(AddGraphicQualitySettings))]
    [SerializeReference] public List<Setting> appliedDisplaySettings;

    private void Start()
    {
        if (settingData == null)
        {
            ThrowUnassignedDisplaySettingsError();
            return;
        }

        foreach (Setting setting in appliedDisplaySettings)
        {
            setting.Initialize();
        }
    }

    [ContextMenu("Add ScreenMode Settings")]
    void AddScreenModeSettings()=> AddSetting<ScreenModeSettingData, ScreenModeSettings>();

    [ContextMenu("Add Resolution Settings")]
    void AddResolutionSettings() => AddSetting<ResolutionSettingsData, ResolutionSettings>();

    [ContextMenu("Add FrameRate Settings")]
    void AddFrameRateSettings() => AddSetting<FrameRateSettingsData,FrameRateSettings>();

    [ContextMenu("Add VSync Settings")]
    void AddVSyncSettings() =>  AddSetting<VSyncSettingsData, VSyncSettings>();

    [ContextMenu("Add Brightness Settings")]
    void AddBrightnessSetting() => AddSetting<BrightnessSettingData, BrightnessSetting>();

    [ContextMenu("Add Graphic Quality Settings")]
    void AddGraphicQualitySettings() => AddSetting<GraphicQualitySettingData, GraphicQualitySetting>();


    // inputs a class type of SettingSO and Setting
    // uses reflection to get the constructor of the Setting class and creates an instance of the Setting class
    void AddSetting<SettingSOType,SettingType>() where SettingSOType : SettingSO  where SettingType : Setting
    {
        if (settingData == null)
        {
            ThrowUnassignedDisplaySettingsError();
            return;
        }
        SettingSOType settingSoData = (SettingSOType)CheckToAddSettingSOToSettingDataHolder<SettingSOType>();
        if (settingSoData == null) return;
        Type settingType = typeof(SettingType);
        ConstructorInfo settingConstructor = settingType.GetConstructor(new Type[] { typeof(SettingSOType) });
        SettingType setting = (SettingType)settingConstructor.Invoke(new object[] { settingSoData });
        appliedDisplaySettings.Add(setting);
    }


    // inputs a class type of SettingsSO
    public SettingSO CheckToAddSettingSOToSettingDataHolder<SettingDataType>() where SettingDataType : SettingSO 
    {
        if (settingData.SettingsSOData.Any(item => item.GetType() == typeof(SettingDataType))) { return null; }
        SettingSO settingSO = (SettingSO)Activator.CreateInstance(typeof(SettingDataType));
        settingData.SettingsSOData.Add(settingSO);
        return settingSO;
    }

    void ThrowUnassignedDisplaySettingsError()
    {
        Debug.LogError("DisplaySettingsConnector: appliedDisplaySettings SO is not assigned.");
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettingsData", menuName = "Settings/AudioSettingsData", order = 1)]
public class AudioSettingsDataHolder : ScriptableObject
{
    [ContextMenuItem("Add Volume Settings", nameof(AddVolumeSettings))]
    [SerializeReference] public List<SettingSO> SettingsSOData = new List<SettingSO>();
    [SerializeField] AudioSettingsConnector settingsConnector;

    private void OnValidate()
    {
        AudioConnectorInitialized();
    }

    [ContextMenu("Add Volume Settings")]
    public void AddVolumeSettings() => AddSettingData<VolumeSettingsData, VolumeSetting>();

    void AddSettingData<SettingSOType, SettingType>() where SettingSOType : SettingSO where SettingType : Setting
    {
        if (!AudioConnectorInitialized()) return;
        SettingSOType settingSoData = Activator.CreateInstance<SettingSOType>();
        if (settingSoData == null) return;
        SettingsSOData.Add(settingSoData);
        Type settingType = typeof(SettingType);
        ConstructorInfo settingConstructor = settingType.GetConstructor(new Type[] { typeof(SettingSOType) });
        SettingType setting = (SettingType)settingConstructor.Invoke(new object[] { settingSoData });
        settingsConnector.appliedDisplaySettings.Add(setting);
    }
    
    bool AudioConnectorInitialized()
    {
        if (settingsConnector == null)
        {
            settingsConnector = FindObjectOfType<AudioSettingsConnector>();
            if (settingsConnector == null) return false;
        }
        return true;
    }
}
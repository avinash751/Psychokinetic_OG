using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

public class AudioSettingsConnector : MonoBehaviour
{
    [ContextMenuItem("Add Volume Setting",nameof(AddVolumeSetting))]
    [SerializeReference] public List<Setting> appliedDisplaySettings;
    public AudioSettingsDataHolder settingData;


    private void Start()
    {
        if(IsAudioSettingsUnAssigned()) return;

        foreach (Setting setting in appliedDisplaySettings)
        {
            setting.Initialize();
        }
    }


    [ContextMenu("Add Volume Setting")]
    public void AddVolumeSetting()
    {
        AddSetting<VolumeSettingsData, VolumeSetting>();
    }


    // inputs a class type of SettingSO and Setting
    // uses reflection to get the constructor of the Setting class and creates an instance of the Setting class
    void AddSetting<SettingSOType, SettingType>() where SettingSOType : SettingSO where SettingType : Setting
    {
        if(IsAudioSettingsUnAssigned()) return;
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
        SettingSO settingSO = (SettingSO)Activator.CreateInstance(typeof(SettingDataType));
        settingData.SettingsSOData.Add(settingSO);
        return settingSO;
    }

    private bool IsAudioSettingsUnAssigned()
    {
        if(settingData == null)
        {
            Debug.LogError("AudioSettingsDataHolder is not assigned to the AudioSettingsConnector");
            return true;
        }
        return false;    
    }
}

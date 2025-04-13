using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VolumeSetting : Setting
{
    public VolumeSettingsData volumeSettingsData;
    public Slider volumeSlider;

    public VolumeSetting(VolumeSettingsData volumeSettingsData)
    {
        this.volumeSettingsData = volumeSettingsData;
    }

    public override void Initialize()
    {
        float currentVolume= volumeSettingsData.GetTrackVolume();
        volumeSlider.value = currentVolume;
        volumeSettingsData.SetTrackVolume(currentVolume);
        ConnectUiToLogic();

    }

    protected override void ConnectUiToLogic()
    {
        UnityAction<float> ChangeVolumeDelegate = value =>
        {
            volumeSettingsData.SetTrackVolume(value);
        };
        volumeSlider.onValueChanged.AddListener(ChangeVolumeDelegate);

    }
}

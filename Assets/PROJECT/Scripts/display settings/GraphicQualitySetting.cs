using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicQualitySetting : Setting
{
    [SerializeField] GraphicQualitySettingData GraphicQualitySettingData;
    [SerializeField] ListSelect GraphicQualitySelection;

    public GraphicQualitySetting(GraphicQualitySettingData dataHolder)
    {
        name = GetType().Name;
        GraphicQualitySettingData = dataHolder;
    }
    public override void Initialize()
    {
        GraphicQualitySelection.OptionList = new List<string>();
        for (int i = 0; i < GraphicQualitySettingData.qualityLevels.Length; i++)
        {
            GraphicQualitySelection.OptionList.Add(GraphicQualitySettingData.qualityLevels[i]);
        }
        GraphicQualitySettingData.GetCurrentQualityLevel(out int qualityLevel);
        int index = GraphicQualitySelection.OptionList.IndexOf(GraphicQualitySettingData.qualityLevels[qualityLevel]);
        GraphicQualitySelection.UpdateIndexAndText(index);
        ConnectUiToLogic();

       
    }

    protected override void ConnectUiToLogic()
    {
        GraphicQualitySelection.OnPress.AddListener(() =>
        {
            GraphicQualitySettingData.SetQualityLevel(GraphicQualitySelection.CurrentIndex);
        });
       
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScreenModeSettingData:SettingSO
{
    public FullScreenMode[] screenModes => new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };

    public ScreenModeSettingData()
    {
        settingName = GetType().Name;
    }
    public void GetScreenMode(out FullScreenMode mode)
    {
        mode = Screen.fullScreenMode;
    }
    public void SetScreenMode(FullScreenMode mode)
    {
        Screen.fullScreenMode = mode;
    }
}

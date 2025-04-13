using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[Serializable]
public abstract class Setting
{  
    public string name;
    public abstract void Initialize();
    protected abstract void ConnectUiToLogic(); 

    
}


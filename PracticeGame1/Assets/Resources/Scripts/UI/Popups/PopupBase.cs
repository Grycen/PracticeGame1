﻿using UnityEngine;
using System.Collections;

public class PopupBase : MonoBehaviour {

    void Awake()
    {
        GameStartupManager.OnEnteringFrontEnd += UpdatePopupData;
    }

    /// <summary>
    /// Update the popup data on entering the FrontEnd or other scenarios here.
    /// </summary>
    public virtual void UpdatePopupData()
    {
        
    }

    void DestroyImmediate()
    {
        GameStartupManager.OnEnteringFrontEnd -= UpdatePopupData;
    }

    void Destroy()
    {
        GameStartupManager.OnEnteringFrontEnd -= UpdatePopupData;
    }
}
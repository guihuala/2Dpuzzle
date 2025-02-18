using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipText : MonoBehaviour
{
    private Text uiText;

    private void Start()
    {
        uiText = GetComponent<Text>();

        ClearText();
        
        EVENTMGR.OnEnterInteractive += UpdateText;
        EVENTMGR.OnExitInteractive += ClearText;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnEnterInteractive -= UpdateText;
        EVENTMGR.OnExitInteractive -= ClearText;
    }

    private void UpdateText(string text)
    {
        uiText.text = text;
    }

    private void ClearText()
    {
        uiText.text = "";
    }
}

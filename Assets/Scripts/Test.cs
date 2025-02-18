using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            AudioManager.Instance.PlaySfx("click");
        
        if (Input.GetKeyDown(KeyCode.Escape))
            UIManager.Instance.OpenPanel("SettingPanel");
    }
}

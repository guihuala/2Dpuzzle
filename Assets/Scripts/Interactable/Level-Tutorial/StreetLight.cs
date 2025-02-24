using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StreetLight : ItemReuiredObject
{
    private Light2D _light;
    
    // 是否开启灯光
    private bool _isLightOn = false;

    protected override void Start()
    {
        base.Start();
        _light = GetComponentInChildren<Light2D>();

        _light.enabled = _isLightOn;
    }

    protected override void ActiveObject()
    {
        base.ActiveObject();
        // 切换灯光状态
        _isLightOn = !_isLightOn;
        _light.enabled = _isLightOn;
    }
}

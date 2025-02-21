using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StreetLight : ItemReuiredObject
{
    private Light2D _light;
    
    protected override void Start()
    {
        base.Start();

        _light = GetComponentInChildren<Light2D>();
        _light.enabled = false;
    }

    protected override void ActiveObject()
    {
        base.ActiveObject();
        
        _light.enabled = true;
    }
}

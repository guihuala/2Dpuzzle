using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 记得修改一下修复的逻辑
public class StreetLight : ItemReuiredObject
{
    [SerializeField] private Light2D _light;
    [SerializeField] private GameObject _particles;
    
    // 是否开启灯光
    private bool _isLightOn = false;

    protected override void Start()
    {
        base.Start();

        _light.enabled = _isLightOn;
    }

    protected override void ActiveObject()
    {
        base.ActiveObject();
        
        // 切换灯光状态
        _isLightOn = !_isLightOn;
        _light.enabled = _isLightOn;
    }

    protected override void FirstTimeActiveObject()
    {
        base.FirstTimeActiveObject();
        
        _particles.SetActive(false);
    }
}

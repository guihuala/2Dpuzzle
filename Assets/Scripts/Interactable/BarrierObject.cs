using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 不需要存储状态
// 在进入场景的时候路灯之类的光源读取数据开启了
// 就会自动切换状态
public class BarrierObject : MonoBehaviour, Interactable
{
    private bool isLit = false;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2D;

    [SerializeField] private Light2D[] targetLights;
    
    [SerializeField]
    [TextArea]
    protected string interactInfo;
    

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        isLit = false;
        foreach (var light in targetLights)
        {
            if (IsInLightRange(light) && light.enabled)
            {
                isLit = true;
                break;
            }
        }
        UpdateState();
    }

    private bool IsInLightRange(Light2D light)
    {
        float distanceToLight = Vector2.Distance(transform.position, light.transform.position);
        return distanceToLight <= light.pointLightOuterRadius;
    }

    private void UpdateState()
    {
        if (isLit)
        {
            spriteRenderer.color = Color.clear;
            collider2D.isTrigger = true;
        }
        else
        {
            spriteRenderer.color = Color.white;
            collider2D.isTrigger = false;
        }
    }

    public virtual void Enter()
    {
        if (!isLit) return;
        EVENTMGR.TriggerEnterInteractive(interactInfo);
    }

    public virtual void Exit()
    {
        if (!isLit) return;
        EVENTMGR.TriggerExitInteractive();
    }

    public void Interact() { }
}

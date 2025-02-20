using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseInteractableObject : MonoBehaviour,Interactable
{
    private bool canInteract = true;  // 是否允许交互
    private bool isLit = false;      // 物体是否在光照范围内
    
    private SpriteRenderer spriteRenderer;

    [SerializeField] 
    [TextArea]protected string interactInfo;
    [SerializeField] protected Material interactOutlineMaterial;
    [SerializeField] protected Material defaultMaterial;
    
    [SerializeField] private bool requiresLightToActivate = true; // 是否需要光照激活交互
    //这边以后可以改成枚举（光照激活、无光照激活、永久激活等等状态）

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Light2D[] lights = FindObjectsOfType<Light2D>();

        isLit = false;

        foreach (var light in lights)
        {
            if (IsInLightRange(light))
            {
                isLit = true;
                break;
            }
        }

        // 根据需要光照的设置更新交互条件
        if (requiresLightToActivate)
        {
            canInteract = isLit;  // 只有在有光照的情况下才能交互
        }
        // else
        // {
        //     canInteract = !isLit;  // 如果不需要光照，则在光照照射时取消激活
        // }
    }

    // 判断物体是否在光源的照射范围内
    private bool IsInLightRange(Light2D light)
    {
        // 获取物体与光源的距离
        float distanceToLight = Vector2.Distance(transform.position, light.transform.position);

        // 判断物体是否在光源的照射范围内
        return distanceToLight <= light.pointLightOuterRadius;
    }

    // 交互事件
    public void Interact()
    {
        if (canInteract)
            Apply();
        else
            Debug.Log("Interaction is disabled, either no light detected or light is required.");
    }

    protected virtual void Apply()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

    public virtual void Enter()
    {
        if (!canInteract)
            return;
        
        EVENTMGR.TriggerEnterInteractive(interactInfo);
        
        if (interactOutlineMaterial != null)
            spriteRenderer.material = interactOutlineMaterial;
    }

    public virtual void Exit()
    {
        if (!canInteract)
            return;
        
        EVENTMGR.TriggerExitInteractive();
        
        if (interactOutlineMaterial != null)
            spriteRenderer.material = defaultMaterial;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool canInteract = true;
    
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] protected string interactInfo;
    [SerializeField] protected Material interactOutlineMaterial;
    [SerializeField] protected Material defaultMaterial;
 
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 交互事件
    public void Interact()
    {
        if(canInteract)
           Apply();
    }

    protected virtual void Apply()
    {
         Debug.Log("Interacted with " + gameObject.name);
    }

    public virtual void Enter()
    {
        EVENTMGR.TriggerEnterInteractive(interactInfo);
        
        spriteRenderer.material = interactOutlineMaterial;
    }

    public virtual void Exit()
    {
        EVENTMGR.TriggerExitInteractive();
        
        spriteRenderer.material = defaultMaterial;
    }
}
